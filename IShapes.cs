using System;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace TextJson
{
    public interface IShape
    {
        string Name { get; }
        double Area { get; }
        ushort Corners { get; }
    }
}