using Zametka;
using System.Text.Json;

namespace LoadFile;

/// <summary>
/// Класс для загрузки данных из файла формата JSON
/// Использую библиотеку System.Text.Json для десериализации данных
/// </summary>
public class JsonLoad : IZamLoader
{
    public List<Zametkapolya> Load(string filePath)
    {
        try
        {   
            // Чтение всего содержимого файла
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