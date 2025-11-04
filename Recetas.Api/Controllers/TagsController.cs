using Microsoft.AspNetCore.Mvc;
using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;

namespace Recetas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetTags()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpPost]
        public async Task<ActionResult<TagDTO>> CreateTag([FromBody] TagDTO tagDto)
        {
            if (string.IsNullOrWhiteSpace(tagDto.Name))
                return BadRequest("El nombre del tag es requerido.");

            // Normalizar nombre a PascalCase
            tagDto.Name = char.ToUpper(tagDto.Name[0]) + tagDto.Name.Substring(1).ToLower();

            var createdTag = await _tagService.CreateTagAsync(tagDto);
            return CreatedAtAction(nameof(GetTags), null, createdTag);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTag(Guid id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}