using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using static System.Console;
namespace TextJson
{
    class Program
    {
        static void Main(string[] args)
        {
            string jsonString =
@"{
  ""Date"": ""2019-08-01T00:00:00"",
  ""Temperature"": 25,
  ""Summary"": ""Hot"",
  ""DatesAvailable"": [
    ""2019-08-01T00:00:00"",
    ""2019-08-02T00:00:00""
  ],
  ""TemperatureRanges"": {
      ""Cold"": {
          ""High"": 20,
          ""Low"": -10
      },
      ""Hot"": {
          ""High"": 60,
          ""Low"": 20
      }
  }
}
";
            jsonString = File.ReadAllText("temperatura.json");
            // Create a JsonNode DOM from a JSON string.
            JsonNode forecastNode = JsonNode.Parse(jsonString);

            // Write JSON from a JsonNode
            var options = new JsonSerializerOptions { WriteIndented = true };
            WriteLine(forecastNode.ToJsonString(options));
            // output:
            //{
            //  "Date": "2019-08-01T00:00:00",
            //  "Temperature": 25,
            //  "Summary": "Hot",
            //  "DatesAvailable": [
            //    "2019-08-01T00:00:00",
            //    "2019-08-02T00:00:00"
            //  ],
            //  "TemperatureRanges": {
            //    "Cold": {
            //      "High": 20,
            //      "Low": -10
            //    },
            //    "Hot": {
            //      "High": 60,
            //      "Low": 20
            //    }
            //  }
            //}

            // Get value from a JsonNode.
            JsonNode temperatureNode = forecastNode["Temperature"];
            WriteLine($"Type={temperatureNode.GetType()}");
            WriteLine($"JSON={temperatureNode.ToJsonString()}");
            //output:
            //Type = System.Text.Json.Nodes.JsonValue`1[System.Text.Json.JsonElement]
            //JSON = 25

            // Get a typed value from a JsonNode.
            int temperatureInt = (int)forecastNode["Temperature"];
            WriteLine($"Value={temperatureInt}");
            //output:
            //Value=25

            // Get a typed value from a JsonNode by using GetValue<T>.
            temperatureInt = forecastNode["Temperature"].GetValue<int>();
            WriteLine($"TemperatureInt={temperatureInt}");
            //output:
            //Value=25

            // Get a JSON object from a JsonNode.
            JsonNode temperatureRanges = forecastNode["TemperatureRanges"];
            WriteLine($"Type={temperatureRanges.GetType()}");
            WriteLine($"JSON={temperatureRanges.ToJsonString()}");
            //output:
            //Type = System.Text.Json.Nodes.JsonObject
            //JSON = { "Cold":{ "High":20,"Low":-10},"Hot":{ "High":60,"Low":20} }

            // Get a JSON array from a JsonNode.
            JsonNode datesAvailable = forecastNode["DatesAvailable"];
            WriteLine($"Type={datesAvailable.GetType()}");
            WriteLine($"JSON={datesAvailable.ToJsonString()}");
            //output:
            //datesAvailable Type = System.Text.Json.Nodes.JsonArray
            //datesAvailable JSON =["2019-08-01T00:00:00", "2019-08-02T00:00:00"]

            // Get an array element value from a JsonArray.
            JsonNode firstDateAvailable = datesAvailable[0];
            WriteLine($"Type={firstDateAvailable.GetType()}");
            WriteLine($"JSON={firstDateAvailable.ToJsonString()}");
            //output:
            //Type = System.Text.Json.Nodes.JsonValue`1[System.Text.Json.JsonElement]
            //JSON = "2019-08-01T00:00:00"

            // Get a typed value by chaining references.
            int coldHighTemperature = (int)forecastNode["TemperatureRanges"]["Cold"]["High"];
            WriteLine($"TemperatureRanges.Cold.High={coldHighTemperature}");
            //output:
            //TemperatureRanges.Cold.High = 20

            // Parse a JSON array
            var datesNode = JsonNode.Parse(@"[""2019-08-01T00:00:00"",""2019-08-02T00:00:00""]");
            JsonNode firstDate = datesNode[0].GetValue<DateTime>();
            WriteLine($"firstDate={ firstDate}");
            //output:
            //firstDate = "2019-08-01T00:00:00"

            JsonObject temperatureRangeObject = forecastNode["TemperatureRanges"].AsObject();
            using var stream = new MemoryStream();
            using var writer = new Utf8JsonWriter(stream);
            temperatureRangeObject.WriteTo(writer);
            writer.Flush();
            TemperatureRanges temperatureRangesArray =
                JsonSerializer.Deserialize<TemperatureRanges>(stream.ToArray());

            foreach (var temperature in temperatureRangesArray)
            {
                Write($"{temperature.Key}.");
                Write($"Low:{temperatureRangesArray[temperature.Key].Low},");
                WriteLine($"High:{temperatureRangesArray[temperature.Key].High}");
            }

            WriteLine($"Cold.Low={temperatureRangesArray["Cold"].Low}, Hot.High={temperatureRangesArray["Hot"].High}");

            
            jsonString = File.ReadAllText("ShapeList.json");
            // Create a JsonNode DOM from a JSON string.

            JsonNode shapeNode = JsonNode.Parse(jsonString);

            JsonNode root = shapeNode.Root;
            JsonArray shapeArray = root["Shapes"].AsArray();
            int count = shapeArray.Count;
            foreach (JsonNode shape in shapeArray)
             {
                 var shapeJsonString = shape.ToJsonString();
                 WriteLine($"Forma recuperada:{shapeJsonString}");
                 new Program().Deserialize(shapeJsonString);
             }

            //new Program().Shapes();
            new FileShapesSource().SetShapesToSource(new FileShapesSource().BuildFileNameOfShape(new Circle(3)     ),new Program().Serialize(new Circle(3)     ) );
            new FileShapesSource().SetShapesToSource(new FileShapesSource().BuildFileNameOfShape(new Rectangle(3,4)),new Program().Serialize(new Rectangle(3,4)) );
            new FileShapesSource().SetShapesToSource(new FileShapesSource().BuildFileNameOfShape(new Square(2,2)   ),new Program().Serialize(new Square(2,2)   ) );
            new FileShapesSource().SetShapesToSource(new FileShapesSource().BuildFileNameOfShape(new Triangle(2,3) ),new Program().Serialize(new Triangle(2,3) ) );


            new Program().Deserialize(new FileShapesSource().GetShapesFromSource("Circle.json"));
            new Program().Deserialize(new FileShapesSource().GetShapesFromSource("Rectangle.json"));
            new Program().Deserialize(new FileShapesSource().GetShapesFromSource("Square.json"));
            new Program().Deserialize(new FileShapesSource().GetShapesFromSource("Triangle.json"));

        }

        public void Shapes()
        {
            double sum = 0;
            int count = 0;

            string jsonString = File.ReadAllText("shapes.json");

            JsonNode document = JsonNode.Parse(jsonString);

            JsonNode root = document.Root;
            JsonArray shapesArray = root["Shapes"].AsArray();

            count = shapesArray.Count;

            foreach (JsonNode shape in shapesArray)
            {
                //shape.get();
                if (shape["rectangle"] is JsonNode typeShape)
                {
                    sum += (double)typeShape;
                }
                else
                {
                    sum += 70;
                }
            }

            double average = sum / count;
            Console.WriteLine($"Average grade : {average}");
        }

        public string Serialize(Shape shape)
        {
            var serializeOptions = new JsonSerializerOptions();
            serializeOptions.Converters.Add(new ShapeConverterWithTypeDiscriminator());
            return  JsonSerializer.Serialize(shape, serializeOptions);
        }
        public IShape Deserialize(string shapeString)
        {
            
            return new JsonSerializerShapes().GetShapesFromJsonString(shapeString);
        }
    }

    public class TemperatureRanges : Dictionary<string, HighLowTemps>
    {

    }

    public class HighLowTemps
    {
        public int High { get; set; }
        public int Low { get; set; }
    }


}
