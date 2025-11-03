using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Recetas.Infrastructure.Repositories
{
    public class RecipeImageRepository : GenericRepository<RecipeImage>, IRecipeImageRepository
    {
        public RecipeImageRepository(RecetasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<RecipeImage>> GetImagesByRecipeIdAsync(Guid recipeId)
        {
            return await _dbSet
                .Where(x => x.RecipeId == recipeId)
                .OrderBy(x => x.Order)
                .ToListAsync();
        }

        public async Task<RecipeImage?> GetImageByIdAsync(Guid imageId)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == imageId);
        }
    }
}
