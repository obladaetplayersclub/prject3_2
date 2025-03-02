using Zametka;
using System.Text.Json;

namespace LoadFile;

public class JsonLoad : IZamLoader
{
    public List<Zametkapolya> Load(string filePath)
    {
        try
        {
            string json = File.ReadAllText(filePath);
            var notes = JsonSerializer.Deserialize<List<Zametkapolya>>(json);
            return notes ?? new List<Zametkapolya>();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при загрузке JSON: " + ex.Message);
            return new List<Zametkapolya>();
        }
    }
}