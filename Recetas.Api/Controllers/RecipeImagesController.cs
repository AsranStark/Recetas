using Microsoft.AspNetCore.Mvc;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api/recipes/{recipeId}/images")]
    public class RecipeImagesController : ControllerBase
    {
        private readonly IRecipeImageService _imageService;

        public RecipeImagesController(IRecipeImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeImage>>> GetImagesByRecipeId(Guid recipeId)
        {
            var images = await _imageService.GetImagesByRecipeIdAsync(recipeId);
            return Ok(images);
        }

        [HttpGet("{imageId}")]
        public async Task<ActionResult<RecipeImage>> GetImageById(Guid recipeId, Guid imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null || image.RecipeId != recipeId)
                return NotFound("Imagen no encontrada.");

            return Ok(image);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeImage>> AddImage(Guid recipeId, [FromBody] AddRecipeImageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
                return BadRequest("La URL de la imagen es requerida.");

            var recipeExists = await _imageService.RecipeExistsAsync(recipeId);
            if (!recipeExists)
                return NotFound("Receta no encontrada.");

            var image = await _imageService.AddImageAsync(recipeId, request.ImageUrl, request.Order ?? 0);
            return CreatedAtAction(nameof(GetImageById), new { recipeId, imageId = image.Id }, image);
        }

        [HttpDelete("{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid recipeId, Guid imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null || image.RecipeId != recipeId)
                return NotFound("Imagen no encontrada.");

            await _imageService.DeleteImageAsync(imageId);
            return NoContent();
        }
    }

    public class AddRecipeImageRequest
    {
        public required string ImageUrl { get; set; }
        public int? Order { get; set; }
    }
}
