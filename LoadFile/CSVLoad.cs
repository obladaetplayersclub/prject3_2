using System.Globalization;
using Zametka;

namespace LoadFile;

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
                
                var parts = line.Split(',');

                if (parts.Length < 4)
                    continue;

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