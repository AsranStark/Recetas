using Microsoft.AspNetCore.Mvc;
using Recetas.Core.Interfaces;
using Recetas.Core.Entities;
using Recetas.Application.DTOs;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IRecipeRepository _recipeRepository;

        public IngredientsController(
            IIngredientRepository ingredientRepository,
            IRecipeRepository recipeRepository)
        {
            _ingredientRepository = ingredientRepository;
            _recipeRepository = recipeRepository;
        }

        [HttpGet("ingredients")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients()
        {
            var ingredients = await _ingredientRepository.GetAllAsync();
            return Ok(ingredients);
        }

        [HttpPost("ingredients")]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredient = new Ingredient 
            { 
                Id = Guid.NewGuid(),
                Name = request.Name,
                IconUrl = request.IconUrl ?? string.Empty
            };

            await _ingredientRepository.AddAsync(ingredient);
            await _ingredientRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        [HttpGet("ingredients/{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredientById(Guid id)
        {
            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(id);
            if (ingredient == null)
                return NotFound("Ingrediente no encontrado.");

            return Ok(ingredient);
        }

        [HttpGet("recipes/{recipeId}/ingredients")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredientsByRecipeId(Guid recipeId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                return NotFound("Receta no encontrada.");

            return Ok(recipe.Ingredients);
        }

        [HttpPost("recipes/{recipeId}/ingredients")]
        public async Task<IActionResult> AddIngredientToRecipe(Guid recipeId, [FromBody] IngredientDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                return NotFound("Receta no encontrada.");

            var ingredient = await _ingredientRepository.GetIngredientByIdAsync(request.IngredientId);
            if (ingredient == null)
                return NotFound("Ingrediente no encontrado.");

            // Verificar que el ingrediente no esté ya en la receta
            if (recipe.Ingredients.Any(i => i.Id == request.IngredientId))
                return BadRequest("Este ingrediente ya está asociado a la receta.");

            recipe.Ingredients.Add(ingredient);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();

            return CreatedAtAction(nameof(GetIngredientsByRecipeId), 
                new { recipeId }, 
                ingredient);
        }

        [HttpDelete("recipes/{recipeId}/ingredients/{ingredientId}")]
        public async Task<IActionResult> RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId)
        {
            var recipe = await _recipeRepository.GetRecipeWithDetailsAsync(recipeId);
            if (recipe == null)
                return NotFound("Receta no encontrada.");

            var ingredient = recipe.Ingredients.FirstOrDefault(i => i.Id == ingredientId);
            if (ingredient == null)
                return NotFound("Ingrediente no encontrado en esta receta.");

            recipe.Ingredients.Remove(ingredient);
            await _recipeRepository.UpdateAsync(recipe);
            await _recipeRepository.SaveChangesAsync();

            return NoContent();
        }
    }

    public class CreateIngredientRequest
    {
        public required string Name { get; set; }
        public string? IconUrl { get; set; }
    }
}

