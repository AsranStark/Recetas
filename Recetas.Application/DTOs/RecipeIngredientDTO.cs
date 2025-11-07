namespace Recetas.Application.DTOs
{
    public class RecipeIngredientDTO
    {
        // Composite key parts
        public Guid IngredientId { get; set; }
        public string IngredientName { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public int UnitCode { get; set; }
        public string UnitName { get; set; } = string.Empty;
    }
}
