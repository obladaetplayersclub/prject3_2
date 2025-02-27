using Zametka;

namespace SaveFile;

public class CSVSave : IZamSaver
{
    public void Save(string filePath, IEnumerable<Zametkapolya> zam)
    {
        try
        {
            var lines = new List<string>();
            foreach (var el in zam)
            {
                string line = $"{el.Id},\"{el.Title}\",\"{el.Words}\",{el.CreateDate:yyyy-MM-dd}";
                lines.Add(line);
            }
            File.WriteAllLines(filePath, lines);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при сохранении CSV: " + ex.Message);
        }
    }
}