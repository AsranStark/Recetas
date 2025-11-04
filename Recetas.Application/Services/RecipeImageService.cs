using Recetas.Core.Entities;
using Recetas.Core.Interfaces;
using Recetas.Application.Interfaces;

namespace Recetas.Application.Services
{
    public class RecipeImageService : IRecipeImageService
    {
        private readonly IRecipeImageRepository _imageRepository;
        private readonly IGenericRepository<Recipe> _recipeRepository;

        public RecipeImageService(
            IRecipeImageRepository imageRepository,
            IGenericRepository<Recipe> recipeRepository)
        {
            _imageRepository = imageRepository;
            _recipeRepository = recipeRepository;
        }

        public async Task<IEnumerable<RecipeImage>> GetImagesByRecipeIdAsync(Guid recipeId)
        {
            return await _imageRepository.GetImagesByRecipeIdAsync(recipeId);
        }

        public async Task<RecipeImage?> GetImageByIdAsync(Guid imageId)
        {
            return await _imageRepository.GetImageByIdAsync(imageId);
        }

        public async Task<RecipeImage> AddImageAsync(Guid recipeId, string imageUrl, int order)
        {
            // Validar que la receta existe
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            if (recipe == null)
                throw new ArgumentException($"Receta con ID {recipeId} no existe.");

            if (string.IsNullOrWhiteSpace(imageUrl))
                throw new ArgumentException("La URL de la imagen no puede estar vacía.");

            var image = new RecipeImage
            {
                Id = Guid.NewGuid(),
                RecipeId = recipeId,
                ImageUrl = imageUrl,
                Order = order
            };

            await _imageRepository.AddAsync(image);
            await _imageRepository.SaveChangesAsync();

            return image;
        }

        public async Task DeleteImageAsync(Guid imageId)
        {
            var image = await _imageRepository.GetImageByIdAsync(imageId);
            if (image == null)
                throw new ArgumentException($"Imagen con ID {imageId} no existe.");

            await _imageRepository.DeleteAsync(image);
            await _imageRepository.SaveChangesAsync();
        }

        public async Task UpdateImageAsync(RecipeImage image)
        {
            await _imageRepository.UpdateAsync(image);
            await _imageRepository.SaveChangesAsync();
        }

        public async Task<bool> RecipeExistsAsync(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetByIdAsync(recipeId);
            return recipe != null;
        }
    }
}
