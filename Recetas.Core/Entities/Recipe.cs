namespace Recetas.Core.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public List<Ingredient> Ingredients { get; set; } = new();
        public ICollection<Step> Steps { get; set; } = new List<Step>();
        public List<Tag> Tags { get; set; } = new();
    }
}