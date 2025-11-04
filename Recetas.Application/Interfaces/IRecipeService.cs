using Recetas.Core.Entities;
using Recetas.Application.DTOs;

namespace Recetas.Application.Interfaces
{
    public interface IRecipeService
    {
        Task<IEnumerable<Recipe>> GetAllRecipesAsync();
        Task<Recipe?> GetRecipeByIdAsync(Guid id);
        Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId);
        Task<Recipe> CreateRecipeAsync(Recipe recipe);
        Task UpdateRecipeAsync(Recipe recipe);
        Task PartialUpdateRecipeAsync(Guid id, PatchRecipeDTO patchDto);
        Task DeleteRecipeAsync(Guid id);
        Task<IEnumerable<Tag>> GetRecipeTagsAsync(Guid recipeId);
        Task AddTagToRecipeAsync(Guid recipeId, string tagName);
        Task RemoveTagFromRecipeAsync(Guid recipeId, Guid tagId);
    }
}
