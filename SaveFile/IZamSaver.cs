using Zametka;

namespace SaveFile;

public interface IZamSaver
{
    void Save(string filePath, IEnumerable<Zametkapolya> notes);
}