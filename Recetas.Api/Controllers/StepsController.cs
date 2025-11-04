using Microsoft.AspNetCore.Mvc;
using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api")]
    public class StepsController : ControllerBase
    {
        private readonly IStepService _stepService;

        public StepsController(IStepService stepService)
        {
            _stepService = stepService;
        }

        [HttpGet("recipes/{recipeId}/steps")]
        public async Task<ActionResult<IEnumerable<Step>>> GetStepsByRecipeId(Guid recipeId)
        {
            try
            {
                var steps = await _stepService.GetStepsByRecipeIdAsync(recipeId);
                return Ok(steps);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("recipes/{recipeId}/steps")]
        public async Task<IActionResult> CreateStep(Guid recipeId, [FromBody] CreateStepDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var step = await _stepService.CreateStepAsync(recipeId, request);
                return CreatedAtAction(nameof(GetStepsByRecipeId), new { recipeId }, step);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("recipes/steps/{stepId}")]
        public async Task<IActionResult> DeleteStep(Guid stepId)
        {
            try
            {
                await _stepService.DeleteStepAsync(stepId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("recipes/steps/{stepId}")]
        public async Task<IActionResult> UpdateStep(Guid stepId, [FromBody] PatchStepDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _stepService.UpdateStepAsync(stepId, request);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
