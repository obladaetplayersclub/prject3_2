using Zametka;

namespace LoadFile;

/// <summary>
/// Интерфейс для загрузки заметок из файла
/// </summary>
public interface IZamLoader
{
    List<Zametkapolya> Load(string filePath);
    
}