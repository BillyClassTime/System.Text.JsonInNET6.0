using System.Text.Json.Serialization;
namespace TextJson
{
    public class Circle : Shape
    {
        [JsonPropertyName("Circumference")]
        public double Circumference { get => 2 * 3.14159 * Radius; }
        public override double Area { get => 3.14159 * (Radius * Radius); }
        public double Radius { get; set; }
        const ushort ZERO = 0;
        public override ushort Corners => ZERO;
        [JsonConstructor]
        public Circle(double radius) 
        {
            Radius = radius;
        
        }
        public Circle() {}
    }
}