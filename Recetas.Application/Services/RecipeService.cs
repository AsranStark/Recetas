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
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;

        public RecipeService(
            IRecipeRepository recipeRepository,
            ITagService tagService,
            IMapper mapper)
        {
            _recipeRepository = recipeRepository;
            _tagService = tagService;
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

        public async Task<IEnumerable<Tag>> GetRecipeTagsAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            return recipe.Tags;
        }

        public async Task AddTagToRecipeAsync(Guid recipeId, string tagName)
        {
            if (string.IsNullOrWhiteSpace(tagName))
                throw new ArgumentException("El nombre del tag no puede estar vacío.");

            // Convertir a PascalCase (primera letra mayúscula)
            tagName = char.ToUpper(tagName[0]) + tagName.Substring(1).ToLower();

            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            // Verificar que el tag no esté ya en la receta
            if (recipe.Tags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Esta etiqueta ya está asociada a la receta.");

            // Obtener o crear el tag
            var tag = await _tagService.GetOrCreateTagByNameAsync(tagName);

            recipe.Tags.Add(tag);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }

        public async Task RemoveTagFromRecipeAsync(Guid recipeId, Guid tagId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                throw new InvalidOperationException("Receta no encontrada.");

            var tag = recipe.Tags.FirstOrDefault(t => t.Id == tagId);
            if (tag == null)
                throw new InvalidOperationException("Etiqueta no encontrada en esta receta.");

            recipe.Tags.Remove(tag);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();
        }
    }
}
