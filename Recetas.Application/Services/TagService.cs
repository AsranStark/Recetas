using AutoMapper;
using Recetas.Application.DTOs;
using Recetas.Application.Interfaces;
using Recetas.Core.Entities;
using Recetas.Core.Interfaces;

namespace Recetas.Application.Services
{
    public class TagService : ITagService
    {
        private readonly IGenericRepository<Tag> _tagRepository;
        private readonly IMapper _mapper;

        public TagService(IGenericRepository<Tag> tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TagDTO>> GetAllTagsAsync()
        {
            var tags = await _tagRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<TagDTO>>(tags);
        }

        public async Task<TagDTO> CreateTagAsync(TagDTO tagDto)
        {
            var tag = _mapper.Map<Tag>(tagDto);
            tag.Id = Guid.NewGuid(); // Asignamos un nuevo GUID
            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync(); // Guardamos los cambios
            return _mapper.Map<TagDTO>(tag);
        }
    }
}