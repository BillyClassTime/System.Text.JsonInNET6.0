# Syste.Text.Json

Vamos a usar esta librearía que ha sido incorporada en .Net Core 3.0 y mejorada en .Net 6

Creamos un proyecto de consola

```
dotnet new console --framework netcoreapp3.1 // Si fuera para 3.1
dotnet new console --framework net5.0 		 // Si fuera para 5.0
dotnet new console --framework net6.0		 // Si fuera para 6.0
```

Añadimos la librearia de Text.Json desde nuget

```
dotnet add package System.Text.Json --version 6.0.0
```

Añadimos un fichero json con las temperaturas:

```json
{
 "Date": "2019-08-01T00:00:00",
 "Temperature": 25,
 "Summary": "Hot",
 "DatesAvailable": [
   "2019-08-01T00:00:00",
   "2019-08-02T00:00:00"
 ],
 "TemperatureRanges": [
   {
   "High": 20,
   "Low": -10
   },
   {
   "High": 60,
   "Low": 20
  }
  ]
}
```

[Search · org:dotnet JsonConverter (github.com)](https://github.com/search?q=org%3Adotnet+JsonConverter&type=discussions)

[How to instantiate JsonSerializerOptions with System.Text.Json | Microsoft Docs](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-configure-options?pivots=dotnet-6-0)

[How to serialize properties of derived classes with System.Text.Json](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-polymorphism)

[What's next for System.Text.Json? - .NET Blog (microsoft.com)](https://devblogs.microsoft.com/dotnet/whats-next-for-system-text-json/)

# Serializando poliformismo

La parte mas interesante sin duda ha sido la serialización de los poliformismos, en una construcción

Interfase -> Clase Base abstracta -> Clase hija

IShape -> Shape -> Circle o IShape -> Shape -> Rectangle, etc

La típica serialización:

```c#
string jsonString = JsonSerializer.Serialize(shape);
```

Para despues deserializar así:

```csharp
JsonSerializer.Deserialize<Shape>(jsonString);
```

No funciona, genera una excepción del tipo:

```
System.NotSupportedException : Deserialization of types without a parameterless constructor, a singular parameterized constructor, or a parameterized constructor annotated with 'JsonConstructorAttribute' is not supported.
```



Para resolver esta excepción debemos utilizar la construcción:

```c#
var serializeOptions = new JsonSerializerOptions();
serializeOptions.Converters.Add(new ShapeConverterWithTypeDiscriminator());
string jsonString = JsonSerializer.Serialize(shape, serializeOptions);
```

Como podemos observar hemos de utilizar opciones Json en la serialización, añadiendo un convertidor (converter) personalizado para que entienda el engine de serialización, que estamos ante una jerarquia de clases.

Para la deserialización hemos de realizar la operación similar pero inversa:

```c#
return JsonSerializer.Deserialize<Shape>(jsonString,serializeOptions);
```



La clase ```ShapeConverterWithTypeDiscriminator``` la hemos construido siguiendo la guia y documentación de Microsoft: [How to write custom converters for JSON serialization - .NET](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-converters-how-to?pivots=dotnet-core-3-1#support-polymorphic-deserialization)

Nuestra clase ha quedado así:

``` c#
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
```



En las pruebas de funcionamiento los Json generados han sido:

Pruebas de serialización: (Llamada al método Write del Converter)

Circle.json

```json
{"TypeDiscriminator":1,"Radius":3,"Name":"TextJson.Circle"}
```



Rectangle.json

```json
{"TypeDiscriminator":2,"Length":3,"Height":4,"Name":"TextJson.Rectangle"}
```



Triangle.json

```json
{"TypeDiscriminator":4,"Width":2,"Height":3,"Name":"TextJson.Triangle"}
```



Square.json

```json
{"TypeDiscriminator":3,"Length":2,"Height":2,"Name":"TextJson.Square"}
```

En las pruebas de deserializción: (Llamada al método Read dle Converter)

Se han creado los objetos Circle, Rectangle, Triangle y Square, respectivamente.

