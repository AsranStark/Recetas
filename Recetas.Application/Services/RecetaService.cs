using Recetas.Application.Interfaces;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;

namespace Recetas.Application.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;

        public RecipeService(IRecipeRepository recipeRepository)
        {
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _recipeRepository.GetRecipesWithDetailsAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
        {
            return await _recipeRepository.GetRecipeWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId)
        {
            return await _recipeRepository.GetRecipesByTagAsync(tagId);
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            await _recipeRepository.AddAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe != null)
            {
                await _recipeRepository.DeleteAsync(recipe);
                await _recipeRepository.SaveChangesAsync();
            }
        }
    }
}