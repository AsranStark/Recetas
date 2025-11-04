using Recetas.Core.Entities;

namespace Recetas.Application.Interfaces
{
    public interface IRecipeImageService
    {
        Task<IEnumerable<RecipeImage>> GetImagesByRecipeIdAsync(Guid recipeId);
        Task<RecipeImage?> GetImageByIdAsync(Guid imageId);
        Task<RecipeImage> AddImageAsync(Guid recipeId, string imageUrl, int order);
        Task UpdateImageAsync(RecipeImage image);
        Task DeleteImageAsync(Guid imageId);
        Task<bool> RecipeExistsAsync(Guid recipeId);
    }
}
