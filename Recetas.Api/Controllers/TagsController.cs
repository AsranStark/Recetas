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
            var createdTag = await _tagService.CreateTagAsync(tagDto);
            return CreatedAtAction(nameof(GetTags), null, createdTag); // Removemos el id de la ruta ya que GetTags no lo usa
        }
    }
}