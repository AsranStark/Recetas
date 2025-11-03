using AutoMapper;
using Recetas.Application.DTOs;
using Recetas.Core.Entities;

namespace Recetas.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Tag, TagDTO>().ReverseMap();
            
            CreateMap<Recipe, RecipeDTO>().ReverseMap();
            CreateMap<CreateRecipeDTO, Recipe>();
            CreateMap<UpdateRecipeDTO, Recipe>();
        }
    }
}