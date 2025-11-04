namespace Recetas.Core.Entities
{
    using System.Text.Json.Serialization;

    public class RecipeImage
    {
        public Guid Id { get; set; }
        public required string ImageUrl { get; set; }
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
        [JsonIgnore]
        public Recipe Recipe { get; set; } = null!;
    }
}
