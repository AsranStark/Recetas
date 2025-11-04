namespace Recetas.Application.DTOs
{
    public class StepDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public int Order { get; set; }
        public Guid RecipeId { get; set; }
    }

    public class CreateStepDTO
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public required int Order { get; set; }
    }

    public class UpdateStepDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public required int Order { get; set; }
    }

    public class PatchStepDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
    }
}
