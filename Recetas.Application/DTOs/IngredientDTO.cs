namespace Recetas.Application.DTOs
{
    public class IngredientDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }

    public class CreateIngredientDTO
    {
        public required string Name { get; set; }
        public string? IconUrl { get; set; }
    }

    public class UpdateIngredientDTO
    {
        public required string Name { get; set; }
        public string IconUrl { get; set; } = string.Empty;
    }

    public class PatchIngredientDTO
    {
        public string? Name { get; set; }
        public string? IconUrl { get; set; }
    }

    public class AddIngredientToRecipeDTO
    {
        public required Guid IngredientId { get; set; }
    }

    // Moved here per request (previously standalone file UpdateRecipeIngredientDTO.cs)
    public class UpdateRecipeIngredientDTO
    {
        public decimal Quantity { get; set; }
        public int UnitCode { get; set; }
    }
}
