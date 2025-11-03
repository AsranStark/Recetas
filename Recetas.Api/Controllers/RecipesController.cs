using Microsoft.AspNetCore.Mvc;
using Recetas.Application.Interfaces;
using Recetas.Application.DTOs;
using AutoMapper;
using Recetas.Core.Entities;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        public RecipesController(IRecipeService recipeService, IMapper mapper)
        {
            _recipeService = recipeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipes()
        {
            var recipes = await _recipeService.GetAllRecipesAsync();
            var dtos = _mapper.Map<IEnumerable<RecipeDTO>>(recipes);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RecipeDTO>> GetRecipe(Guid id)
        {
            var recipe = await _recipeService.GetRecipeByIdAsync(id);
            if (recipe == null)
            {
                return NotFound("Receta no encontrada.");
            }
            var dto = _mapper.Map<RecipeDTO>(recipe);
            return Ok(dto);
        }

        [HttpGet("tag/{tagId}")]
        public async Task<ActionResult<IEnumerable<RecipeDTO>>> GetRecipesByTag(Guid tagId)
        {
            var recipes = await _recipeService.GetRecipesByTagAsync(tagId);
            var dtos = _mapper.Map<IEnumerable<RecipeDTO>>(recipes);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<RecipeDTO>> CreateRecipe([FromBody] CreateRecipeDTO createRecipeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var recipe = _mapper.Map<Recipe>(createRecipeDto);
            var createdRecipe = await _recipeService.CreateRecipeAsync(recipe);
            var dto = _mapper.Map<RecipeDTO>(createdRecipe);
            
            return CreatedAtAction(nameof(GetRecipe), new { id = createdRecipe.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRecipe(Guid id, [FromBody] UpdateRecipeDTO updateRecipeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var recipe = _mapper.Map<Recipe>(updateRecipeDto);
            recipe.Id = id;

            await _recipeService.UpdateRecipeAsync(recipe);
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PartialUpdateRecipe(Guid id, [FromBody] PatchRecipeDTO patchRecipeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _recipeService.PartialUpdateRecipeAsync(id, patchRecipeDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRecipe(Guid id)
        {
            await _recipeService.DeleteRecipeAsync(id);
            return NoContent();
        }
    }
}
