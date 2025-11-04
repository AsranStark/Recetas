namespace Recetas.Core.Entities
{
    using System.Text.Json.Serialization;

    public class Step
    {
        public Guid Id { get; set; }
        public required string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
        [JsonIgnore]
        public Recipe Recipe { get; set; } = null!;
    }
}