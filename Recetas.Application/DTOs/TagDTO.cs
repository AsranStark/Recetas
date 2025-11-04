namespace Recetas.Application.DTOs
{
    public class TagDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
    }

    public class AddTagToRecipeDTO
    {
        public required string Name { get; set; }
    }
}