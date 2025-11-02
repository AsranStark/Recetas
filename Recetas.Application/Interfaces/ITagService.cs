using Recetas.Application.DTOs;

namespace Recetas.Application.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTagsAsync();
        Task<TagDTO> CreateTagAsync(TagDTO tagDto);
    }
}