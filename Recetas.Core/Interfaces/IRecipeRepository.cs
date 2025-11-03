using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    public interface IRecipeRepository : IGenericRepository<Recipe>
    {
        Task<IEnumerable<Recipe>> GetRecipesWithDetailsAsync();
        Task<Recipe?> GetRecipeWithDetailsAsync(Guid id);
        Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId);
    }
}
