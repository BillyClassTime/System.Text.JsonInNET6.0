namespace TextJson
{
    public class Rectangle : Shape
    {
        public double Perimeter { get => 2 * (Length + Height); }

        public override double Area { get => Length * Height; }

        public double Length { get; set; }
        public double Height { get; set; }
        const ushort FOUR = 4;
        public override ushort Corners => FOUR;


        public Rectangle(double length, double height)
        {
            Height = height;
            Length = length;
        }
        public Rectangle(){}

    }
}