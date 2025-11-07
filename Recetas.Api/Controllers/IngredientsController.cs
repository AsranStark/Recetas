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

        [HttpGet("recipes/{recipeId}/recipeingredients")]
        public async Task<ActionResult<IEnumerable<RecipeIngredientDTO>>> GetIngredientsByRecipeId(Guid recipeId)
        {
            try
            {
                var items = await _ingredientService.GetIngredientsByRecipeIdAsync(recipeId);
                return Ok(items);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // New endpoint returning only base ingredients (no quantities/units)
        [HttpGet("recipes/{recipeId}/ingredients")]
        public async Task<ActionResult<IEnumerable<IngredientDTO>>> GetBaseIngredientsByRecipeId(Guid recipeId)
        {
            try
            {
                var items = await _ingredientService.GetBaseIngredientsByRecipeIdAsync(recipeId);
                return Ok(items);
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
                var list = await _ingredientService.GetIngredientsByRecipeIdAsync(recipeId);
                return CreatedAtAction(nameof(GetIngredientsByRecipeId), new { recipeId }, list);
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

        [HttpPatch("recipes/{recipeId}/ingredients/{ingredientId}")]
        public async Task<IActionResult> UpdateRecipeIngredient(Guid recipeId, Guid ingredientId, [FromBody] UpdateRecipeIngredientDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _ingredientService.UpdateRecipeIngredientAsync(recipeId, ingredientId, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

