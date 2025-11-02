namespace Recetas.Core.Entities
{
    public class Step
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
        public Recipe Recipe { get; set; } = null!;
    }
}