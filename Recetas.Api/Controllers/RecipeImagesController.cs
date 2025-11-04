using Microsoft.AspNetCore.Mvc;
using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class RecipeImagesController : ControllerBase
    {
        private readonly IRecipeImageService _imageService;

        public RecipeImagesController(IRecipeImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("recipes/{recipeId}/images")]
        public async Task<ActionResult<IEnumerable<RecipeImage>>> GetImagesByRecipeId(Guid recipeId)
        {
            var images = await _imageService.GetImagesByRecipeIdAsync(recipeId);
            return Ok(images);
        }

        [HttpGet("recipes/images/{imageId}")]
        public async Task<ActionResult<RecipeImage>> GetImageById(Guid imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null)
                return NotFound("Imagen no encontrada.");

            return Ok(image);
        }

        [HttpPost("recipes/{recipeId}/images")]
        public async Task<ActionResult<RecipeImage>> AddImage(Guid recipeId, [FromBody] CreateRecipeImageDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.ImageUrl))
                return BadRequest("La URL de la imagen es requerida.");

            var recipeExists = await _imageService.RecipeExistsAsync(recipeId);
            if (!recipeExists)
                return NotFound("Receta no encontrada.");

            var image = await _imageService.AddImageAsync(recipeId, request.ImageUrl, request.Order ?? 0);
            return CreatedAtAction(nameof(GetImageById), new { imageId = image.Id }, image);
        }

        [HttpDelete("recipes/images/{imageId}")]
        public async Task<IActionResult> DeleteImage(Guid imageId)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null)
                return NotFound("Imagen no encontrada.");

            await _imageService.DeleteImageAsync(imageId);
            return NoContent();
        }

        [HttpPatch("recipes/images/{imageId}")]
        public async Task<IActionResult> UpdateImage(Guid imageId, [FromBody] PatchRecipeImageDTO request)
        {
            var image = await _imageService.GetImageByIdAsync(imageId);
            if (image == null)
                return NotFound("Imagen no encontrada.");

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                image.ImageUrl = request.ImageUrl;

            if (request.Order.HasValue)
                image.Order = request.Order.Value;

            await _imageService.UpdateImageAsync(image);
            return NoContent();
        }
    }
}
