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
            CreateMap<Ingredient, IngredientDTO>().ReverseMap();
            CreateMap<RecipeImageDTO, RecipeImage>().ReverseMap();

            CreateMap<RecipeIngredient, RecipeIngredientDTO>()
                .ForMember(d => d.IngredientId, o => o.MapFrom(s => s.IngredientId))
                .ForMember(d => d.IngredientName, o => o.MapFrom(s => s.Ingredient != null ? s.Ingredient.Name : string.Empty))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.Quantity))
                .ForMember(d => d.UnitCode, o => o.MapFrom(s => s.MeasurementUnitCode))
                .ForMember(d => d.UnitName, o => o.MapFrom(s => s.MeasurementUnit != null ? s.MeasurementUnit.Name : string.Empty));

            CreateMap<Recipe, RecipeDTO>()
                .ForMember(d => d.Ingredients, o => o.MapFrom(s => s.RecipeIngredients))
                .ForMember(d => d.Images, o => o.MapFrom(s => s.Images))
                .ReverseMap();
            CreateMap<CreateRecipeDTO, Recipe>();
            CreateMap<UpdateRecipeDTO, Recipe>();

            CreateMap<Recipe, RecipeListDTO>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Images.OrderBy(i => i.Order).FirstOrDefault()));
        }
    }
}