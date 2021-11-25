using System.IO;
namespace TextJson
{
    public class FileShapesSource
    {
        public string GetShapesFromSource(string fileName) => File.ReadAllText(fileName);

        public void SetShapesToSource(string fileName, string shapeData) => File.WriteAllText(fileName, shapeData);
    }

    public static class FileExtensionShape
    {
        public static string BuildFileNameOfShape(this FileShapesSource file, Shape shape)
        {
            string fileName = default;
            if (shape.Name.IndexOf(".") != -1)
                fileName = shape.Name.Substring(shape.Name.IndexOf(".") + 1);
            else
                fileName = shape.Name;
            return fileName = $"{fileName}.json";

        }
    }
}