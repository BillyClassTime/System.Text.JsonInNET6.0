using System.Text.Json;
using System.Text.Json.Serialization;
namespace TextJson
{
    public abstract class Shape : IShape
    {
        public string Name {get; } 
        public Shape(){
            this.Name = this.GetType().ToString();
        }

        abstract public double Area { get; }

        abstract public ushort Corners { get; }
    }
}
