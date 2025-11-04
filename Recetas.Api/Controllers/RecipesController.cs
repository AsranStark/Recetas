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

        public RecipesController(
            IRecipeService recipeService,
            IMapper mapper)
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

        [HttpGet("{recipeId}/tags")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetRecipeTags(Guid recipeId)
        {
            try
            {
                var tags = await _recipeService.GetRecipeTagsAsync(recipeId);
                var tagDtos = _mapper.Map<IEnumerable<TagDTO>>(tags);
                return Ok(tagDtos);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("{recipeId}/tags")]
        public async Task<IActionResult> AddTagToRecipe(Guid recipeId, [FromBody] AddTagToRecipeDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _recipeService.AddTagToRecipeAsync(recipeId, request.Name);
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

        [HttpDelete("{recipeId}/tags/{tagId}")]
        public async Task<IActionResult> RemoveTagFromRecipe(Guid recipeId, Guid tagId)
        {
            try
            {
                await _recipeService.RemoveTagFromRecipeAsync(recipeId, tagId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
