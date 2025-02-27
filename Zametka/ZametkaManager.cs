using System.Dynamic;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace Zametka;

public class ZametkaManager
{
   public List<Zametkapolya> Zametki { get; private set; } = new();
   public string FilePath { get; private set; }

   public ZametkaManager(string filePath)
   {
      FilePath = filePath;
   }

   public void Zagruzka()
   {
      try
      {
         if (!File.Exists(FilePath))
         {
            Console.WriteLine("Файл не найден. Введи адрес заново");
            return;
         }

         string[] lines = File.ReadAllLines(FilePath);
         foreach (var elemt in lines)
         {  
            var matches = Regex.Matches(elemt, @"\[(.*?)\]");
            if (matches.Count == 4)
            {
               int id = int.Parse(matches[0].Groups[1].Value);
               string title = matches[1].Groups[1].Value;
               string words = matches[2].Groups[1].Value;
               DateTime createDate = DateTime.Parse(matches[3].Groups[1].Value);
               Zametki.Add(new Zametkapolya
               {
                  Id = id,
                  Title = title,
                  Words = words,
                  CreateDate = createDate
                     
               });

            }
         }
      }
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
      
   }

   public void SaveZametki()
   {
      try
      {
         List<string> lines = new();
         foreach (var el in Zametki)
         {
            string line =
               $"[{el.Id}] [{el.Title}] [{el.Words}] [{el.CreateDate:yyyy-MM-dd}]";
            lines.Add(line);
         }
         File.WriteAllLines(FilePath, lines);
         
      } 
      catch (Exception e)
      {
         Console.WriteLine(e);
         throw;
      }
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