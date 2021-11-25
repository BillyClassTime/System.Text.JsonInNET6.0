using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace TextJson
{
    public class JsonSerializerShapes
    {
        public IShape GetShapesFromJsonString(string shapesJson)
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new ShapeConverterWithTypeDiscriminator());
            return JsonSerializer.Deserialize<Shape>(shapesJson,serializeOptions);
            //return JsonSerializer.Deserialize<Shape>(shapesJson);

        }
    }
}