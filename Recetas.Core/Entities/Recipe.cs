namespace Recetas.Core.Entities
{
    public class Recipe
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? VideoUrl { get; set; }
        public int? Difficulty { get; set; } 
        public int? Rating { get; set; }
        public int? PreparationTime { get; set; } // in minutes
        public bool PreparationInAdvance { get; set; } = false;
        public List<Ingredient> Ingredients { get; set; } = new();
        public ICollection<Step> Steps { get; set; } = new List<Step>();
        public List<Tag> Tags { get; set; } = new();
    }
}