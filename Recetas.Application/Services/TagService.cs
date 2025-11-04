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

        public async Task<Tag> GetOrCreateTagByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del tag no puede estar vacío.");

            // Convertir a PascalCase (primera letra mayúscula)
            name = char.ToUpper(name[0]) + name.Substring(1).ToLower();

            var allTags = await _tagRepository.GetAllAsync();
            var tag = allTags.FirstOrDefault(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (tag == null)
            {
                tag = new Tag { Id = Guid.NewGuid(), Name = name };
                await _tagRepository.AddAsync(tag);
                await _tagRepository.SaveChangesAsync();
            }

            return tag;
        }

        public async Task DeleteTagAsync(Guid id)
        {
            var tag = await _tagRepository.GetByIdAsync(id);
            if (tag == null)
                throw new InvalidOperationException("Tag no encontrado.");

            await _tagRepository.DeleteAsync(tag);
            await _tagRepository.SaveChangesAsync();
        }
    }
}