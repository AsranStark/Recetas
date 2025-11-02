using Recetas.Core.Entities;

namespace Recetas.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(Guid id);
        Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId);
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task DeleteRecipeAsync(Guid id);
    }
}