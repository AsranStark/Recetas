using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Recetas.Core.Entities;

namespace Recetas.Infrastructure.Data.Seeding;

public static class JsonSeeder
{
    public static async Task SeedAsync(RecetasDbContext db, string contentRootPath, ILogger logger)
    {
        // Para entorno local: asegura la BD si no existe (sin depender de migraciones)
        await db.Database.EnsureCreatedAsync();

        var seedDir = Path.GetFullPath(Path.Combine(contentRootPath, "..", "Recetas.Infrastructure", "Data", "Seed"));
        if (!Directory.Exists(seedDir))
        {
            logger.LogWarning("Seed dir no encontrado: {SeedDir}", seedDir);
            return;
        }

        // Orden recomendado: bases primero
        await SeedIfEmpty<Ingredient>(db, Path.Combine(seedDir, "ingredients.json"), logger);
        await SeedIfEmpty<Tag>(db, Path.Combine(seedDir, "tags.json"), logger);
        await SeedIfEmpty<Recipe>(db, Path.Combine(seedDir, "recipes.json"), logger);
        // Entidades dependientes
        await SeedIfEmpty<Step>(db, Path.Combine(seedDir, "steps.json"), logger);
        await SeedIfEmpty<RecipeImage>(db, Path.Combine(seedDir, "images.json"), logger);

        // Vincular relaciones N-N desde archivo de enlaces
        await LinkRecipeRelationsAsync(db, Path.Combine(seedDir, "recipe-links.json"), logger);

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

    private class RecipeLinksByNameDto
    {
        public string RecipeName { get; set; } = string.Empty;
        public List<string> IngredientNames { get; set; } = new();
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
            var allIngredients = await db.Ingredients.AsNoTracking().ToListAsync();
            var ingByName = allIngredients
                .GroupBy(i => Norm(i.Name))
                .ToDictionary(g => g.Key, g => g.First());

            var allTags = await db.Tags.AsNoTracking().ToListAsync();
            var tagByName = allTags
                .GroupBy(t => Norm(t.Name))
                .ToDictionary(g => g.Key, g => g.First());

            foreach (var link in byName)
            {
                var recipe = await db.Recipes
                    .Include(r => r.Ingredients)
                    .Include(r => r.Tags)
                    .FirstOrDefaultAsync(r => r.Name == link.RecipeName);

                if (recipe is null)
                {
                    logger.LogWarning("Recipe '{RecipeName}' no existe. Enlaces omitidos.", link.RecipeName);
                    continue;
                }

                if (link.IngredientNames.Count > 0)
                {
                    var wanted = link.IngredientNames.Select(Norm).ToHashSet();
                    var resolved = wanted
                        .Select(n => ingByName.TryGetValue(n, out var i) ? i : null)
                        .Where(i => i != null)
                        .Select(i => i!)
                        .ToList();
                    foreach (var ing in resolved)
                    {
                        if (!recipe.Ingredients.Any(x => x.Id == ing.Id))
                            recipe.Ingredients.Add(ing);
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
                .Include(r => r.Ingredients)
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
                    if (!recipe.Ingredients.Any(x => x.Id == ing.Id))
                        recipe.Ingredients.Add(ing);
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
