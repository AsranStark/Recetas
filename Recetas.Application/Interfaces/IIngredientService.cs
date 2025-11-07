using Recetas.Application.DTOs;
using Recetas.Core.Entities;

namespace Recetas.Application.Interfaces
{
    public interface IIngredientService
    {
        Task<IEnumerable<Ingredient>> GetAllIngredientsAsync();
        Task<Ingredient> CreateIngredientAsync(CreateIngredientDTO createIngredientDto);
        Task<Ingredient?> GetIngredientByIdAsync(Guid id);
    Task<IEnumerable<RecipeIngredientDTO>> GetIngredientsByRecipeIdAsync(Guid recipeId);
    Task<IEnumerable<IngredientDTO>> GetBaseIngredientsByRecipeIdAsync(Guid recipeId);
        Task AddIngredientToRecipeAsync(Guid recipeId, Guid ingredientId);
        Task RemoveIngredientFromRecipeAsync(Guid recipeId, Guid ingredientId);
        Task UpdateRecipeIngredientAsync(Guid recipeId, Guid ingredientId, UpdateRecipeIngredientDTO updateDto);
    }
}
