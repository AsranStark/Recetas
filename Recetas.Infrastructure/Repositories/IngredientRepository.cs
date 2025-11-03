using Microsoft.EntityFrameworkCore;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Infrastructure.Data;

namespace Recetas.Infrastructure.Repositories
{
    public class IngredientRepository : GenericRepository<Ingredient>, IIngredientRepository
    {
        public IngredientRepository(RecetasDbContext context) : base(context)
        {
        }

        public async Task<Ingredient?> GetIngredientByIdAsync(Guid id)
        {
            return await _context.Ingredients
                .FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
