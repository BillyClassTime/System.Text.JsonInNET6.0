using System;

namespace TextJson
{
    public class Triangle : Shape
    {
        public double Perimeter { get => Height + Width + (Math.Sqrt(Math.Pow(Height, 2) + Math.Pow(Width, 2))); }
        public override double Area { get => 0.5 * Height * Width; }
        public double Width { get; set; }
        public double Height { get; set; }
        const ushort THREE = 3;
        public override ushort Corners => THREE;

        public Triangle(double width, double height)
        {
            Width = width;
            Height = height;
        }
        public Triangle() {}
    }
}