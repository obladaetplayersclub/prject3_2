using System.Dynamic;
using System.Text.RegularExpressions;
using Spectre.Console;
using Zametka;
using LoadFile;
using SaveFile;

namespace ZametkaMan;

public class ZametkaManager
{
   public List<Zametkapolya> Zametki { get; private set; } = new ();
   public string FilePath { get; private set; }
   public string SavePath { get; set; }
   
   private IZamLoader _loader;
   private IZamSaver _saver;

   public ZametkaManager(string loadFilePath, string saveFilePath, IZamLoader loader, IZamSaver saver)
   {
      FilePath = loadFilePath;
      SavePath = saveFilePath;
      _loader = loader;
      _saver = saver;
   }
   

   public void Zagruzka()
   {
      if (File.Exists(FilePath))
      {
         Zametki = _loader.Load(FilePath);
         AnsiConsole.MarkupLine("[green]Заметки успешно загружены![/]");
      }
      else
      {
         AnsiConsole.MarkupLine("[red]Файл не найден.[/]");
         Zametki = new List<Zametkapolya>();
      }
   }

   public void SaveZametki()
   {
      _saver.Save(SavePath, Zametki);
      AnsiConsole.MarkupLine("[green]Заметки успешно сохранены![/]");
   }

   public int GiveId()
   {
      return Zametki.Any() ? Zametki.Max(n => n.Id) + 1 : 1;
   }

   public Zametkapolya? GetById(int id)
   {
      return Zametki.FirstOrDefault(n => n.Id == id);
   }
}