using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TextJson
{
    public class ShapeConverterWithTypeDiscriminator : JsonConverter<Shape>
    {
        enum TypeDiscriminator
        {
            Circle = 1,
            Rectangle = 2,
            Square = 3,
            Triangle = 4
        }

        public override bool CanConvert(Type typeToConvert) =>
            typeof(Shape).IsAssignableFrom(typeToConvert);

        public override Shape Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            string propertyName = reader.GetString();
            if (propertyName != "TypeDiscriminator")
            {
                throw new JsonException();
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.Number)
            {
                throw new JsonException();
            }

            TypeDiscriminator typeDiscriminator = (TypeDiscriminator)reader.GetInt32();
            Shape shape = typeDiscriminator switch
            {
                TypeDiscriminator.Circle => new Circle(),
                TypeDiscriminator.Rectangle => new Rectangle(),
                TypeDiscriminator.Triangle => new Triangle(),
                TypeDiscriminator.Square => new Square(),
                _ => throw new JsonException()
            };

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return shape;
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    propertyName = reader.GetString();
                    reader.Read();
                    switch (propertyName)
                    {
                        case "Length":
                            double length = reader.GetDouble();
                            switch (typeDiscriminator)
                            {
                                case TypeDiscriminator.Rectangle:
                                    ((Rectangle)shape).Length = length;
                                    break;
                                case TypeDiscriminator.Square:
                                    ((Square)shape).Length = length;
                                    break;
                            }
                            break;
                        case "Height":
                            double height = reader.GetDouble();
                            switch (typeDiscriminator)
                            {
                                case TypeDiscriminator.Rectangle:
                                    ((Rectangle)shape).Height = height;
                                    break;
                                case TypeDiscriminator.Square:
                                    ((Square)shape).Height = height;
                                    break;
                                case TypeDiscriminator.Triangle:
                                    ((Triangle)shape).Height = height;
                                    break;
                            }
                            break;
                        case "Width":
                            double width = reader.GetDouble();
                            ((Triangle)shape).Width = width;
                            break;
                        case "Radius":
                            double radius = reader.GetDouble();
                            ((Circle)shape).Radius = radius;
                            break;
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, Shape shape, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            if (shape is Circle circle)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Circle);
                writer.WriteNumber("Radius", circle.Radius);
            }
            else if (shape is Rectangle rectangle)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Rectangle);
                writer.WriteNumber("Length", rectangle.Length);
                writer.WriteNumber("Height", rectangle.Height);
            }
            else if (shape is Square square)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Square);
                writer.WriteNumber("Length", square.Length);
                writer.WriteNumber("Height", square.Height);
            }
            else if (shape is Triangle triangle)
            {
                writer.WriteNumber("TypeDiscriminator", (int)TypeDiscriminator.Triangle);
                writer.WriteNumber("Width", triangle.Width);
                writer.WriteNumber("Height", triangle.Height);
            }
            writer.WriteString("Name",shape.Name);
            writer.WriteEndObject();
        }
    }
}