using Recetas.Application.Interfaces;
using Recetas.Application.DTOs;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using AutoMapper;

namespace Recetas.Application.Services
{
    public class RecipeService : IRecipeService
    {
        private readonly IRecipeRepository _recipeRepository;
        private readonly IMapper _mapper;

        public RecipeService(IRecipeRepository recipeRepository, IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<Recipe>> GetAllRecipesAsync()
        {
            return await _recipeRepository.GetRecipesWithDetailsAsync();
        }

        public async Task<Recipe?> GetRecipeByIdAsync(Guid id)
        {
            return await _recipeRepository.GetRecipeWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Recipe>> GetRecipesByTagAsync(Guid tagId)
        {
            return await _recipeRepository.GetRecipesByTagAsync(tagId);
        }

        public async Task<Recipe> CreateRecipeAsync(Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.Name))
                throw new ArgumentException("El nombre de la receta es requerido.");

            recipe.Id = Guid.NewGuid();
            await _recipeRepository.AddAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
            return recipe;
        }

        public async Task UpdateRecipeAsync(Recipe recipe)
        {
            if (string.IsNullOrWhiteSpace(recipe.Name))
                throw new ArgumentException("El nombre de la receta es requerido.");

            var existing = await _recipeRepository.GetByIdAsync(recipe.Id);
            if (existing == null)
                throw new ArgumentException($"Receta con ID {recipe.Id} no encontrada.");

            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task PartialUpdateRecipeAsync(Guid id, PatchRecipeDTO patchDto)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe == null)
                throw new ArgumentException($"Receta con ID {id} no encontrada.");

            // Actualizar solo los campos que vienen en el patch (no-null)
            if (!string.IsNullOrWhiteSpace(patchDto.Name))
                recipe.Name = patchDto.Name;
            
            if (patchDto.Description != null)
                recipe.Description = patchDto.Description;
            
            if (patchDto.VideoUrl != null)
                recipe.VideoUrl = patchDto.VideoUrl;
            
            if (patchDto.Difficulty.HasValue)
                recipe.Difficulty = patchDto.Difficulty;
            
            if (patchDto.Rating.HasValue)
                recipe.Rating = patchDto.Rating;
            
            if (patchDto.PreparationTime.HasValue)
                recipe.PreparationTime = patchDto.PreparationTime;
            
            if (patchDto.PreparationInAdvance.HasValue)
                recipe.PreparationInAdvance = patchDto.PreparationInAdvance.Value;

            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task DeleteRecipeAsync(Guid id)
        {
            var recipe = await _recipeRepository.GetByIdAsync(id);
            if (recipe != null)
            {
                await _recipeRepository.DeleteAsync(recipe);
                await _recipeRepository.SaveChangesAsync();
            }
        }
    }
}
