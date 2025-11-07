using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Seeding;

public static class JsonSeeder
{
    public static async Task SeedAsync(RecetasDbContext db, string contentRootPath, ILogger logger)
    {
        // La creación/migración de la BD se realiza en el arranque (Program.cs)

        var seedDir = Path.GetFullPath(Path.Combine(contentRootPath, "..", "Recetas.Infrastructure", "Data", "Seed"));
        if (!Directory.Exists(seedDir))
        {
            logger.LogWarning("Seed dir no encontrado: {SeedDir}", seedDir);
            return;
        }

    // Orden recomendado: catálogos base primero
    await SeedIfEmpty<MeasurementUnit>(db, Path.Combine(seedDir, "units.json"), logger);
    await SeedIfEmpty<Ingredient>(db, Path.Combine(seedDir, "ingredients.json"), logger);
        await SeedIfEmpty<Tag>(db, Path.Combine(seedDir, "tags.json"), logger);
        await SeedIfEmpty<Recipe>(db, Path.Combine(seedDir, "recipes.json"), logger);
        // Entidades dependientes
        await SeedIfEmpty<Step>(db, Path.Combine(seedDir, "steps.json"), logger);
        await SeedIfEmpty<RecipeImage>(db, Path.Combine(seedDir, "images.json"), logger);

    // Guardar antes de vincular para que las consultas por nombre encuentren datos
    await db.SaveChangesAsync();
    // Evitar conflictos de tracking entre entidades seed y entidades que se cargarán para vincular
    db.ChangeTracker.Clear();

        // Vincular relaciones N-N desde archivo de enlaces
        await LinkRecipeRelationsAsync(db, Path.Combine(seedDir, "recipe-links.json"), logger);

        // Guardar vínculos
        await db.SaveChangesAsync();
    }

    private static async Task SeedIfEmpty<T>(RecetasDbContext db, string filePath, ILogger logger) where T : class
    {
        if (!File.Exists(filePath))
        {
            logger.LogInformation("Seed file no encontrado: {File}", filePath);
            return;
        }

        var set = db.Set<T>();
        if (await set.AnyAsync())
        {
            logger.LogInformation("Tabla {Entity} ya contiene datos. Seeding omitido.", typeof(T).Name);
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);
        var items = JsonSerializer.Deserialize<List<T>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (items is { Count: > 0 })
        {
            await set.AddRangeAsync(items);
            logger.LogInformation("Seed insertado: {Count} filas en {Entity}", items.Count, typeof(T).Name);
        }
    }

    private class RecipeLinksDto
    {
        public Guid RecipeId { get; set; }
        public List<Guid> IngredientIds { get; set; } = new();
        public List<Guid> TagIds { get; set; } = new();
    }

    private class IngredientLinkDto
    {
        public string Name { get; set; } = string.Empty; // ingredient name
        public decimal Quantity { get; set; } // amount
        public int UnitCode { get; set; } // MeasurementUnit.Code
    }

    private class RecipeLinksByNameDto
    {
        public string RecipeName { get; set; } = string.Empty;
        public List<IngredientLinkDto> Ingredients { get; set; } = new();
        public List<string> TagNames { get; set; } = new();
    }

    private static async Task LinkRecipeRelationsAsync(RecetasDbContext db, string filePath, ILogger logger)
    {
        if (!File.Exists(filePath))
        {
            logger.LogInformation("Mapping de relaciones no encontrado: {File}", filePath);
            return;
        }

        var json = await File.ReadAllTextAsync(filePath);

        // Intentar primero por nombres (nuevo formato)
        var byName = JsonSerializer.Deserialize<List<RecipeLinksByNameDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (byName is not null && byName.Count > 0 && !string.IsNullOrWhiteSpace(byName[0].RecipeName))
        {
            string Norm(string s) => s.Trim().ToLowerInvariant();

            // Cargar catálogos a memoria para emparejar por nombre de forma robusta
            var allIngredients = await db.Ingredients.ToListAsync();
            var ingByName = allIngredients
                .GroupBy(i => Norm(i.Name))
                .ToDictionary(g => g.Key, g => g.First());

            var allTags = await db.Tags.ToListAsync();
            var tagByName = allTags
                .GroupBy(t => Norm(t.Name))
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var link in byName)
            {
                var recipe = await db.Recipes
                    .Include(r => r.RecipeIngredients)
                    .Include(r => r.Tags)
                    .FirstOrDefaultAsync(r => r.Name.ToLower() == link.RecipeName.ToLower());

                if (recipe is null)
                {
                    logger.LogWarning("Recipe '{RecipeName}' no existe. Enlaces omitidos.", link.RecipeName);
                    continue;
                }

                if (link.Ingredients.Count > 0)
                {
                    var wanted = link.Ingredients.Select(il => Norm(il.Name)).ToHashSet();
                    var resolved = link.Ingredients
                        .Select(il => (dto: il, entity: ingByName.TryGetValue(Norm(il.Name), out var ing) ? ing : null))
                        .Where(x => x.entity != null)
                        .Select(x => (x.dto, x.entity!))
                        .ToList();
                    foreach (var (dto, ing) in resolved)
                    {
                        if (!recipe.RecipeIngredients.Any(ri => ri.IngredientId == ing.Id))
                        {
                            db.RecipeIngredients.Add(new RecipeIngredient
                            {
                                RecipeId = recipe.Id,
                                IngredientId = ing.Id,
                                Quantity = dto.Quantity,
                                MeasurementUnitCode = dto.UnitCode
                            });
                        }
                    }
                    var missing = wanted.Where(n => !ingByName.ContainsKey(n)).ToList();
                    if (missing.Count > 0)
                        logger.LogWarning("Ingredientes no encontrados para '{Recipe}': {Missing}", recipe.Name, string.Join(", ", missing));
                }

                if (link.TagNames.Count > 0)
                {
                    var wanted = link.TagNames.Select(Norm).ToHashSet();
                    var resolved = wanted
                        .Select(n => tagByName.TryGetValue(n, out var t) ? t : null)
                        .Where(t => t != null)
                        .Select(t => t!)
                        .ToList();
                    foreach (var tag in resolved)
                    {
                        if (!recipe.Tags.Any(x => x.Id == tag.Id))
                            recipe.Tags.Add(tag);
                    }
                    var missing = wanted.Where(n => !tagByName.ContainsKey(n)).ToList();
                    if (missing.Count > 0)
                        logger.LogWarning("Tags no encontrados para '{Recipe}': {Missing}", recipe.Name, string.Join(", ", missing));
                }
            }
            return;
        }

        // Fallback: antiguo formato por GUIDs
        var byIds = JsonSerializer.Deserialize<List<RecipeLinksDto>>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new List<RecipeLinksDto>();

        foreach (var link in byIds)
        {
            var recipe = await db.Recipes
                .Include(r => r.RecipeIngredients)
                .Include(r => r.Tags)
                .FirstOrDefaultAsync(r => r.Id == link.RecipeId);

            if (recipe is null)
            {
                logger.LogWarning("Recipe {RecipeId} no existe. Enlaces omitidos.", link.RecipeId);
                continue;
            }

            if (link.IngredientIds.Count > 0)
            {
                var ings = await db.Ingredients
                    .Where(i => link.IngredientIds.Contains(i.Id))
                    .ToListAsync();
                foreach (var ing in ings)
                {
                    if (!recipe.RecipeIngredients.Any(x => x.IngredientId == ing.Id))
                    {
                        db.RecipeIngredients.Add(new RecipeIngredient
                        {
                            RecipeId = recipe.Id,
                            IngredientId = ing.Id,
                            Quantity = 0m,
                            MeasurementUnitCode = 1 // default unidades si formato antiguo
                        });
                    }
                }
            }

            if (link.TagIds.Count > 0)
            {
                var tags = await db.Tags
                    .Where(t => link.TagIds.Contains(t.Id))
                    .ToListAsync();
                foreach (var tag in tags)
                {
                    if (!recipe.Tags.Any(x => x.Id == tag.Id))
                        recipe.Tags.Add(tag);
                }
            }
        }
    }
}
