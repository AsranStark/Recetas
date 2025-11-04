using Recetas.Application.DTOs;
using Recetas.Core.Entities;

namespace Recetas.Application.Interfaces
{
    public interface IStepService
    {
        Task<IEnumerable<Step>> GetStepsByRecipeIdAsync(Guid recipeId);
        Task<Step> CreateStepAsync(Guid recipeId, CreateStepDTO createStepDto);
        Task UpdateStepAsync(Guid stepId, PatchStepDTO patchStepDto);
        Task DeleteStepAsync(Guid stepId);
    }
}
