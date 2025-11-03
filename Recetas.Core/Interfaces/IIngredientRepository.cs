using Recetas.Core.Entities;

namespace Recetas.Core.Interfaces
{
    public interface IIngredientRepository : IGenericRepository<Ingredient>
    {
        Task<Ingredient?> GetIngredientByIdAsync(Guid id);
    }
}
