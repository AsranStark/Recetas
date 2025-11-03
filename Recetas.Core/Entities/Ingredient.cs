namespace Recetas.Core.Entities
{
    public class Ingredient
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }
}