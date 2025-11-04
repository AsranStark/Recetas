using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;

namespace Recetas.Application.Services
{
    public class StepService : IStepService
    {
        private readonly IGenericRepository<Step> _stepRepository;
        private readonly IRecipeRepository _recipeRepository;

        public StepService(
            IGenericRepository<Step> stepRepository,
            IRecipeRepository recipeRepository)
        {
            _stepRepository = stepRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<Step>> GetStepsByRecipeIdAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            return recipe.Steps.OrderBy(s => s.Order).ToList();
        }

        public async Task<Step> CreateStepAsync(Guid recipeId, CreateStepDTO createStepDto)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var step = new Step
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                Name = createStepDto.Name,
                Description = createStepDto.Description ?? string.Empty,
                Order = createStepDto.Order
            };

            await _stepRepository.AddAsync(step);
            await _stepRepository.SaveChangesAsync();

            return step;
        }

        public async Task UpdateStepAsync(Guid stepId, PatchStepDTO patchStepDto)
        {
            var step = await _stepRepository.GetByIdAsync(stepId);
            if (step == null)
                throw new InvalidOperationException("Step no encontrado.");

            if (!string.IsNullOrWhiteSpace(patchStepDto.Name))
                step.Name = patchStepDto.Name;

            if (!string.IsNullOrWhiteSpace(patchStepDto.Description))
                step.Description = patchStepDto.Description;

            if (patchStepDto.Order.HasValue)
                step.Order = patchStepDto.Order.Value;

            await _stepRepository.UpdateAsync(step);
            await _stepRepository.SaveChangesAsync();
        }

        public async Task DeleteStepAsync(Guid stepId)
        {
            var step = await _stepRepository.GetByIdAsync(stepId);
            if (step == null)
                throw new InvalidOperationException("Step no encontrado.");

            await _stepRepository.DeleteAsync(step);
            await _stepRepository.SaveChangesAsync();
        }
    }
}
