using Zametka;

namespace LoadFile;

public interface IZamLoader
{
    List<Zametkapolya> Load(string filePath);
    
}