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

        public IngredientService(
            IIngredientRepository ingredientRepository,
            IRecipeRepository recipeRepository)
        {
            _ingredientRepository = ingredientRepository;
            _recipeRepository = recipeRepository;
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

        public async Task<IEnumerable<Ingredient>> GetIngredientsByRecipeIdAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            return recipe.Ingredients;
        }

        public async Task AddIngredientToRecipeAsync(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingrediente no encontrado.");

            // Verificar que el ingrediente no esté ya en la receta
            if (recipe.Ingredients.Any(i => i.Id == ingredientId))
                throw new InvalidOperationException("Este ingrediente ya está asociado a la receta.");

            recipe.Ingredients.Add(ingredient);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task RemoveIngredientFromRecipeAsync(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient == null)
                throw new InvalidOperationException("Ingrediente no encontrado en esta receta.");

            recipe.Ingredients.Remove(ingredient);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }
    }
}
