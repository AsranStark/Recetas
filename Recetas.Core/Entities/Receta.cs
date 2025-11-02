namespace Recetas.Core.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new();
        public List<Step> Steps { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
    }
}