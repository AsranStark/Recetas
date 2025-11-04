namespace Recetas.Application.DTOs
{
    public class RecipeImageDTO
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
    }

    public class CreateRecipeImageDTO
    {
        public required string ImageUrl { get; set; }
        public int? Order { get; set; }
    }

    public class UpdateRecipeImageDTO
    {
        public required string ImageUrl { get; set; }
        public int Order { get; set; }
    }

    public class PatchRecipeImageDTO
    {
        public string? ImageUrl { get; set; }
        public int? Order { get; set; }
    }
}
