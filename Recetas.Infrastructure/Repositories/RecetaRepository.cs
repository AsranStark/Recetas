using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    public class RecipeRepository : GenericRepository<Recipe>, IRecipeRepository
    {
        public RecipeRepository(RecetasDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Recipe>> GetRecipesWithDetailsAsync()
        {
            return await _context.Recipes
                .Include(r => r.Tags)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .ToListAsync();
        }

        public async Task<Recipe?> GetRecipeWithDetailsAsync(Guid id)
        {
            return await _context.Recipes
                .Include(r => r.Tags)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId)
        {
            return await _context.Recipes
                .Include(r => r.Tags)
                .Include(r => r.Ingredients)
                .Include(r => r.Steps)
                .Where(r => r.Tags.Any(t => t.Id == tagId))
                .ToListAsync();
        }
    }
}