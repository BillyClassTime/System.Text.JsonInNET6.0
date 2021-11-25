using System.IO;
namespace TextJson
{
    public class FileShapesSource
    {
        public string GetShapesFromSource(string fileName) => File.ReadAllText(fileName);
    }
}