using Zametka;
using System.Text.Json;
namespace SaveFile;

public class JSONSave : IZamSaver
{
    public void Save(string filePath, IEnumerable<Zametkapolya> notes)
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(notes, options);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении JSON: " + ex.Message);
        }
    }
}