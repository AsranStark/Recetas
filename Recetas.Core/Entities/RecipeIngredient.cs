namespace Recetas.Core.Entities
{
    public class RecipeIngredient
    {
        public Guid RecipeId { get; set; }
        public Guid IngredientId { get; set; }
        public decimal Quantity { get; set; }
        public int MeasurementUnitCode { get; set; }

        public Recipe? Recipe { get; set; }
        public Ingredient? Ingredient { get; set; }
        public MeasurementUnit? MeasurementUnit { get; set; }
    }
}
