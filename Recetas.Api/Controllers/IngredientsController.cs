using Microsoft.AspNetCore.Mvc;
using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class IngredientsController : ControllerBase
    {
        private readonly IIngredientService _ingredientService;

        public IngredientsController(IIngredientService ingredientService)
        {
            _ingredientService = ingredientService;
        }

        [HttpGet("ingredients")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetAllIngredients()
        {
            var ingredients = await _ingredientService.GetAllIngredientsAsync();
            return Ok(ingredients);
        }

        [HttpPost("ingredients")]
        public async Task<IActionResult> CreateIngredient([FromBody] CreateIngredientDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ingredient = await _ingredientService.CreateIngredientAsync(request);
            return CreatedAtAction(nameof(GetIngredientById), new { id = ingredient.Id }, ingredient);
        }

        [HttpGet("ingredients/{id}")]
        public async Task<ActionResult<Ingredient>> GetIngredientById(Guid id)
        {
            var ingredient = await _ingredientService.GetIngredientByIdAsync(id);
            if (ingredient == null)
                return NotFound("Ingrediente no encontrado.");

            return Ok(ingredient);
        }

        [HttpGet("recipes/{recipeId}/ingredients")]
        public async Task<ActionResult<IEnumerable<Ingredient>>> GetIngredientsByRecipeId(Guid recipeId)
        {
            try
            {
                var ingredients = await _ingredientService.GetIngredientsByRecipeIdAsync(recipeId);
                return Ok(ingredients);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("recipes/{recipeId}/ingredients")]
        public async Task<IActionResult> AddIngredientToRecipe(Guid recipeId, [FromBody] AddIngredientToRecipeDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ingredientService.AddIngredientToRecipeAsync(recipeId, request.IngredientId);
                var ingredient = await _ingredientService.GetIngredientByIdAsync(request.IngredientId);
                return CreatedAtAction(nameof(GetIngredientsByRecipeId), 
                    new { recipeId }, 
                    ingredient);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("recipes/{recipeId}/ingredients/{ingredientId}")]
        public async Task<IActionResult> RemoveIngredientFromRecipe(Guid recipeId, Guid ingredientId)
        {
            try
            {
                await _ingredientService.RemoveIngredientFromRecipeAsync(recipeId, ingredientId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

