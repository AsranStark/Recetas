namespace Recetas.Core.Entities
{
    public class RecipeImage
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}
