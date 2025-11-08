namespace Recetas.Application.DTOs
{
    public class RecipeDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }
        public int? PreparationTime { get; set; }
        public bool PreparationInAdvance { get; set; }
        public List<RecipeIngredientDTO> Ingredients { get; set; } = new();
        public List<TagDTO> Tags { get; set; } = new();
        public List<RecipeImageDTO> Images { get; set; } = new();
    }

    public class RecipeListDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int? Rating { get; set; }
        public List<TagDTO> Tags { get; set; } = new();
        public RecipeImageDTO? Image { get; set; }
    }

    public class CreateRecipeDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }
        public int? PreparationTime { get; set; }
        public bool PreparationInAdvance { get; set; }
    }

    public class UpdateRecipeDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }
        public int? PreparationTime { get; set; }
        public bool PreparationInAdvance { get; set; }
    }

    public class PatchRecipeDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? VideoUrl { get; set; }
        public int? Difficulty { get; set; }
        public int? Rating { get; set; }
        public int? PreparationTime { get; set; }
        public bool? PreparationInAdvance { get; set; }
    }
}
