namespace Recetas.Core.Entities
{
    public class MeasurementUnit
    {
        // Primary Key
        public int Code { get; set; }

        // Display name to return in endpoints
        public string Name { get; set; } = string.Empty;
    }
}
