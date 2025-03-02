using System.Globalization;
using Zametka;

namespace LoadFile;

/// <summary>
/// Класс для загрузки заметок из CSV файла.
/// Ожидаемый формат строки: id (целое число), "title"(строка), "words"(строка), yyyy-MM-dd(DateTime)
/// </summary>
public class CsvLoad : IZamLoader
{
    public List<Zametkapolya> Load(string filePath)
    {
        var zam = new List<Zametkapolya>();
        try
        {
            foreach (var line in File.ReadAllLines(filePath))
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;
                
                // Разбиваем строку по запятым
                var parts = line.Split(',');
                
                // Если строка не соответствует ожидаемому формату, пропускаем её
                if (parts.Length < 4)
                    continue;
                
                // Дальше преобразуем элементы строки:
                if (!int.TryParse(parts[0], out int id))
                    continue;
                string title = parts[1].Trim('\"');
                string words = parts[2].Trim('\"');
                if (!DateTime.TryParseExact(parts[3], "yyyy-MM-dd", CultureInfo.InvariantCulture,
                        DateTimeStyles.None, out DateTime createDate))
                {
                    continue;
                }
                
                zam.Add(new Zametkapolya()
                {
                    Id = id,
                    Title = title,
                    Words = words,
                    CreateDate = createDate
                });
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        return zam;
    }
    
}