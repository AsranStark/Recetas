using Recetas.Application.DTOs;
using Recetas.Core.Entities;

namespace Recetas.Application.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDTO>> GetAllTagsAsync();
        Task<TagDTO> CreateTagAsync(TagDTO tagDto);
        Task<Tag> GetOrCreateTagByNameAsync(string name);
        Task DeleteTagAsync(Guid id);
    }
}