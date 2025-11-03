using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    public interface IRecipeImageRepository : IGenericRepository<RecipeImage>
    {
        Task<IEnumerable<RecipeImage>> GetImagesByRecipeIdAsync(Guid recipeId);
        Task<RecipeImage?> GetImageByIdAsync(Guid imageId);
    }
}
