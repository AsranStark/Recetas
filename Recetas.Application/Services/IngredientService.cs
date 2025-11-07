using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;

namespace Recetas.Application.Services
{
    public class IngredientService : IIngredientService
    {
    private readonly IIngredientRepository _ingredientRepository;
    private readonly IRecipeRepository _recipeRepository;
    private readonly AutoMapper.IMapper _mapper;

        public IngredientService(
            IIngredientRepository ingredientRepository,
            IRecipeRepository recipeRepository,
            AutoMapper.IMapper mapper)
        {
            _ingredientRepository = ingredientRepository;
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync()
        {
            return await _ingredientRepository.GetAllAsync();
        }

        public async Task<Ingredient> CreateIngredientAsync(CreateIngredientDTO createIngredientDto)
        {
            var ingredient = new Ingredient
            {
                Id = Guid.NewGuid(),
                Name = createIngredientDto.Name,
                IconUrl = createIngredientDto.IconUrl ?? string.Empty
            };

            await _ingredientRepository.AddAsync(ingredient);
            await _ingredientRepository.SaveChangesAsync();

            return ingredient;
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(Guid id)
        {
            return await _ingredientRepository.GetIngredientByIdAsync(id);
        }

        public async Task<IEnumerable<RecipeIngredientDTO>> GetIngredientsByRecipeIdAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            // Mapear a DTOs detallados con cantidad y unidad
            var detailed = _mapper.Map<IEnumerable<RecipeIngredientDTO>>(recipe.RecipeIngredients);
            return detailed;
        }

        public async Task<IEnumerable<IngredientDTO>> GetBaseIngredientsByRecipeIdAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var ingredients = recipe.RecipeIngredients
                .Select(ri => ri.Ingredient!)
                .Where(i => i != null)!;

            return _mapper.Map<IEnumerable<IngredientDTO>>(ingredients);
        }

        public async Task AddIngredientToRecipeAsync(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingrediente no encontrado.");

            if (recipe.RecipeIngredients.Any(ri => ri.IngredientId == ingredientId))
                throw new InvalidOperationException("Este ingrediente ya está asociado a la receta.");

            recipe.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId = recipeId,
                IngredientId = ingredientId,
                Quantity = 0m,
                MeasurementUnitCode = 1 // Unidades por defecto
            });

            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task RemoveIngredientFromRecipeAsync(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var link = recipe.RecipeIngredients.FirstOrDefault(ri => ri.IngredientId == ingredientId);
            if (link == null)
                throw new InvalidOperationException("Ingrediente no encontrado en esta receta.");

            recipe.RecipeIngredients.Remove(link);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task UpdateRecipeIngredientAsync(Guid recipeId, Guid ingredientId, UpdateRecipeIngredientDTO updateDto)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var link = recipe.RecipeIngredients.FirstOrDefault(ri => ri.IngredientId == ingredientId);
            if (link == null)
                throw new InvalidOperationException("Ingrediente no encontrado en esta receta.");

            link.Quantity = updateDto.Quantity;
            link.MeasurementUnitCode = updateDto.UnitCode;

            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }
    }
}
