using Zametka;

namespace SaveFile;

/// <summary>
/// Интерфейс для сохранения заметок в файл
/// </summary>
public interface IZamSaver
{
    void Save(string filePath, IEnumerable<Zametkapolya> notes);
}