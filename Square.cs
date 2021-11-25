using System;

namespace TextJson
{
    public class Square : Shape
    {

        public double Perimeter { get => 2 * (Length + Height); }
        public override double Area { get => Length * Height; }
        public double Length { get; set; }
        public double Height { get; set; }
        const ushort FOUR = 4;
        public override ushort Corners => FOUR;

        public Square(double length, double height)
        {
            if (length != height)
                throw new ArgumentException("The lengh must be same to height");
            Height = height;
            Length = length;
        }
        public Square() {}

    }
}