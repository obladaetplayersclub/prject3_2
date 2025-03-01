using System.Dynamic;
using System.Text.RegularExpressions;
using Spectre.Console;
using Zametka;
using LoadFile;
using SaveFile;

namespace ZametkaMan;

public class ZametkaManager
{
   public List<Zametkapolya> Zametki { get; set; } = new ();
   public string FilePath { get; private set; }
   public string SavePath { get; set; }
   
   private IZamLoader _zagr;
   private IZamSaver _sohr;

   public ZametkaManager(string loadFilePath, string saveFilePath, IZamLoader loD, IZamSaver saV)
   {
      FilePath = loadFilePath;
      SavePath = saveFilePath;
      _zagr = loD;
      _sohr = saV;
   }
   

   public void Zagruzka()
   {
      if (File.Exists(FilePath))
      {
         Zametki = _zagr.Load(FilePath);
         AnsiConsole.MarkupLine("[green]Заметки успешно загружены![/]");
      }
      else
      {
         AnsiConsole.MarkupLine("[red]Файл не найден.[/]");
         Zametki = new List<Zametkapolya>();
      }
   }

   public void SohrZametki()
   {
      _sohr.Save(SavePath, Zametki);
      AnsiConsole.MarkupLine("[green]Заметки успешно сохранены![/]");
   }

   public int GiveId()
   {
      if (Zametki == null || Zametki.Count == 0)
         return 1;

      int maxId = Zametki[0].Id;
      foreach (var zam in Zametki)
      {
         if (zam.Id > maxId)
            maxId = zam.Id;
      }
      return maxId + 1;
   }

   public Zametkapolya? GetById(int id)
   {
      foreach (var zam in Zametki)
      {
         if (zam.Id == id)
            return zam;
      }
      return null;
   }
}