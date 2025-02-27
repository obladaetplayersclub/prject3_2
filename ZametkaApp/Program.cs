using Zametka;
using ZametkaMan;
using Spectre.Console;
using LoadFile;
using SaveFile;

namespace ZametkaApp;

internal class Program
{
    static void Main(string[] args)
    {
        AnsiConsole.MarkupLine("[blue]Выберите формат файла для загрузки заметок:[/]");
        AnsiConsole.MarkupLine("1. CSV");
        AnsiConsole.MarkupLine("2. JSON");
        string? loadChoice = Console.ReadLine();
        string defaultLoadPath = (loadChoice == "2") ? "notes.json" : "notes.csv";
        string loadFilePath = AskForFilePath($"[blue]Введите путь к файлу для загрузки заметок (по умолчанию {defaultLoadPath}):[/]", defaultLoadPath);
        
        AnsiConsole.MarkupLine("[blue]Выберите формат файла для сохранения заметок:");
        AnsiConsole.MarkupLine("1. CSV");
        AnsiConsole.MarkupLine("2. JSON");
        string? saveChoice = Console.ReadLine();
        string defaultSavePath = (saveChoice == "2") ? "notes.json" : "notes.csv";
        string saveFilePath = AskForFilePath($"[blue]Введите путь к файлу для сохранения заметок (по умолчанию {defaultSavePath}):[/]", defaultSavePath);

        IZamLoader loader = (loadChoice == "2") 
            ? new JsonLoad() 
            : new CsvLoad();
        IZamSaver saver = (saveChoice == "2") 
            ? new JSONSave() 
            : new CSVSave();

        var zametkaManager = new ZametkaManager(loadFilePath, saveFilePath, loader, saver);
        zametkaManager.Zagruzka();
        bool exit = false;
        while (!exit)
        {
            AnsiConsole.MarkupLine("\n[bold yellow]Менеджер личных заметок[/]");
            AnsiConsole.MarkupLine("[blue]Выберите действие:[/]");
            AnsiConsole.MarkupLine("1. Просмотр списка заметок");
            AnsiConsole.MarkupLine("2. Просмотр детальной информации о заметке");
            AnsiConsole.MarkupLine("3. Добавление новой заметки");
            AnsiConsole.MarkupLine("4. Сортировка заметок");
            AnsiConsole.MarkupLine("5. Фильтрация заметок");
            AnsiConsole.MarkupLine("6. Редактирование заметки");
            AnsiConsole.MarkupLine("7. Удалить заметку");
            AnsiConsole.MarkupLine("8. Вывести таблицу с заметками");
            AnsiConsole.MarkupLine("9. Вывести календарь с заметками");
            AnsiConsole.MarkupLine("10. Поиск синонимов для слова");
            AnsiConsole.MarkupLine("11. Синхронизация с облаком");
            AnsiConsole.MarkupLine("12. Сохранить заметки в файл");
            AnsiConsole.MarkupLine("0. Выход");
            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    WatchZam(zametkaManager);
                    break;
                case "2":
                    DetailedInf(zametkaManager);
                    break;
                case "3":
                    DobZam(zametkaManager);
                    break;
                case "4":
                    break;
                case "5":
                    break;
                case "6":
                    break;
                case "7":
                    UdZam(zametkaManager);
                    break;
                case "8":
                    SozdTabl(zametkaManager);
                    break;
                case "9":
                    break;
                case "10":
                    break;
                case "11":
                    break;
                case "12":
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Введено некорректное значение! Нажмите любую клавишу для выхода[/]");
                    Console.ReadKey();
                    break;
                    
            }
        }
    }
    
    private static string AskForFilePath(string prompt, string? defaultPath)
    {
        while (true)
        {
            AnsiConsole.MarkupLine(prompt);
            string? path = Console.ReadLine()?.Trim();
            if (!string.IsNullOrWhiteSpace(path))
            {
                return path;
            }
            if (!string.IsNullOrWhiteSpace(defaultPath))
            {
                return defaultPath;
            }
            AnsiConsole.MarkupLine("[red]Путь не может быть пустым![/]");
        }
    }

    static void WatchZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow]Ты выбрал \"Просмотр списка заметок\"[/]");
        foreach (var zam in zametkaManager.Zametki)
        {
            AnsiConsole.MarkupLine($"[green]ID:[/] {zam.Id}  [green]Заголовок:[/] {zam.Title}  [green]Дата создания:[/] {zam.CreateDate:yyyy-MM-dd}");
        }
    }

    static void DetailedInf(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Просмотр информации о записке\". Введи ID заметки:[/]");
        string nom = Console.ReadLine();
        if (int.TryParse(nom, out int id))
        {
            var note = zametkaManager.GetById(id);
            if (note != null)
            {
                AnsiConsole.MarkupLine($"\n[bold underline]Детальная информация о заметке {note.Id}[/]");
                AnsiConsole.MarkupLine($"[green]Заголовок:[/] {note.Title}");
                AnsiConsole.MarkupLine($"[green]Содержимое:[/]\n{note.Words}");
                AnsiConsole.MarkupLine($"[green]Дата создания:[/] {note.CreateDate:yyyy-MM-dd}");
            }

            else
            {
                AnsiConsole.MarkupLine($"[red bold] Отсутствует такой ID[/] {id}");
            }
        }

        else
        {
            AnsiConsole.MarkupLine("Ты ввел не ID - элемент должен быть числом");
        }
    }

    private static void DobZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Добавление новой заметки\"[/]");
        AnsiConsole.MarkupLine("Напиши заголовок");
        string title = Console.ReadLine();
        AnsiConsole.MarkupLine("Напиши текст данной заметки");
        string words = Console.ReadLine();
        var newZam = new Zametkapolya
        {
            Id = zametkaManager.GiveId(),
            Title = title,
            Words = words,
            CreateDate = DateTime.Now
        };
        zametkaManager.Zametki.Add(newZam);
        AnsiConsole.MarkupLine("[green] Ты добавил новую заметку, поздравляю[/]");
    }

    private static void UdZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Добавление новой заметки\"[/]");
        string ud = Console.ReadLine();
        if (int.TryParse(ud, out int udNumb))
        {   
            var zam = zametkaManager.GetById(udNumb);
            if (zam != null)
            {
                zametkaManager.Zametki.Remove(zam);
                AnsiConsole.MarkupLine("[green] Заметка удалена[/]");

            }
            else
            {
                AnsiConsole.MarkupLine("[red] Заметка не была найдена! Возможно вы ввели не тот ID[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine("[red] Вводимое значение должно быть целочисленным![/]");
        }
    }

    static void SozdTabl(ZametkaManager zametkaManager)
    {
        var tabl = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Chartreuse3)
            .AddColumn("Id")
            .AddColumn("Заголовок")
            .AddColumn("Текст")
            .AddColumn("Дата создания");
        foreach (var el in zametkaManager.Zametki)
        {
            tabl.AddRow(el.Id.ToString(), el.Title, el.Words, el.CreateDate.ToString("yyyy-MM-dd"));
        }
        AnsiConsole.Write(tabl);
    }

    // static void SozdCal(ZametkaManager zametkaManager)
    // {
    //     var zametk = zametkaManager.Zametki
    //         .GroupBy(n => n.CreateDate.Date)
    //         .ToDictionary(g => g.Key, g => g.Count());
    //     int startYear = 2015;
    //     int endYear = DateTime.Now.Year;
    //     for (int year = startYear; year <= endYear; year++)
    //     {
    //         for (int month = 1; month <= 12; month++)
    //         {
    //             var cal = new Calendar(year, month)
    //                 .Culture("ru-RU")
    //                 .HeaderStyle(Style.Parse("blue bold"));
    //             foreach (var el in zametk.Keys)
    //             {
    //                 if (el.Year == year && el.Month == month)
    //                 {
    //                     cal.AddCalendarEvent(el);
    //                     if (zametk[el] == 0)
    //                     {
    //                         cal.Highlight(el, new Style(Color.White));
    //                     }
    //                     else if (zametk[el] == 1)
    //                     {
    //                         cal.Highlight(el, new Style(Color.Yellow));
    //                     }
    //                     else if (zametk[el] > 1)
    //                     {
    //                         cal.Highlight(el, new Style(Color.Yellow, decoration: Decoration.Bold));
    //                     }
    //                 }
    //             }
    //         }
    //     }
        
}



