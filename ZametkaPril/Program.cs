using System.Linq.Expressions;
using Zametka;
using ZametkaMan;
using Spectre.Console;
using LoadFile;
using SaveFile;
using System.Text.RegularExpressions;
using System.Text.Json;

namespace ZametkaApp;

/// <summary>
/// Основной класс, который нужен для работы моего консольного приложения
/// </summary>
internal class Program
{
    static void Main(string[] args)
    {
        // Запрос формата файла для загрузки
        AnsiConsole.MarkupLine("[blue]Выберите формат файла для загрузки заметок:[/]");
        AnsiConsole.MarkupLine("1. CSV");
        AnsiConsole.MarkupLine("2. JSON");
        string? loadChoice = Console.ReadLine();
        string defaultLoadPath = (loadChoice == "2") ? "notes.json" : "notes.csv";
        string loadFilePath =
            AskForFilePath($"[blue]Введите путь к файлу для загрузки заметок:[/]",
                defaultLoadPath);
        
        // Запрос формата файла для сохранения заметок
        AnsiConsole.MarkupLine("[blue]Выберите формат файла для сохранения заметок:[/]");
        AnsiConsole.MarkupLine("1. CSV");
        AnsiConsole.MarkupLine("2. JSON");
        string? saveChoice = Console.ReadLine();
        string defaultSavePath = (saveChoice == "2") ? "notes.json" : "notes.csv";
        string saveFilePath =
            AskForFilePath($"[blue]Введите путь к файлу для сохранения заметок:[/]",
                defaultSavePath);
        
        // Инициализация "загрузчика" и "сохранителя" на основании выбранного формата
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
            AnsiConsole.MarkupLine("1. Просмотр всех заметок");
            AnsiConsole.MarkupLine("2. Просмотр информации о заметке по ее номеру");
            AnsiConsole.MarkupLine("3. Добавить заметку (номер генерируется автоматически)");
            AnsiConsole.MarkupLine("4. Сортировка заметок");
            AnsiConsole.MarkupLine("5. Фильтрация заметок");
            AnsiConsole.MarkupLine("6. Редактирование заметки");
            AnsiConsole.MarkupLine("7. Удаление заметки");
            AnsiConsole.MarkupLine("8. Вывод таблицы с заметками");
            AnsiConsole.MarkupLine("9. Вывод календаря с заметками");
            AnsiConsole.MarkupLine("10. Поиск синонимов для слова");
            AnsiConsole.MarkupLine("11. Синхронизация");
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
                    Sort(zametkaManager);
                    break;
                case "5":
                    Filtr(zametkaManager);
                    break;
                case "6":
                    Izm(zametkaManager);
                    break;
                case "7":
                    UdZam(zametkaManager);
                    break;
                case "8":
                    SozdTabl(zametkaManager);
                    break;
                case "9":
                    SozdCal(zametkaManager);
                    break;
                case "10":
                    IskSyn(zametkaManager).GetAwaiter().GetResult();
                    break;
                case "11":
                    SyncGoogleDrive();
                    break;
                case "12":
                    zametkaManager.SohrZametki();
                    AnsiConsole.MarkupLine($"[green]Данные сохранены в файл: {zametkaManager.SavePath}[/]");
                    AnsiConsole.MarkupLine($"[green] Спасибо за использование моего приложения![/]");
                    exit = true;
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
    
    /// <summary>
    /// Запрашивает у пользователя путь к файлу заметок. Если пользователь вводит пустую строку,
    /// возвращается значение по умолчанию
    /// </summary>
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
    
    /// <summary>
    /// Выводит список всех заметок в консоль.
    /// </summary>
    static void WatchZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow]Ты выбрал \"Просмотр всех заметок\"[/]");
        foreach (var zam in zametkaManager.Zametki)
        {
            AnsiConsole.MarkupLine(
                $"[green]ID:[/] {zam.Id}  [red]Заголовок:[/] {zam.Title} [yellow]Текст:[/] {zam.Words}  [blue]Дата создания:[/] {zam.CreateDate:yyyy-MM-dd}");
        }
    }
    
    /// <summary>
    /// Выводит детальную информацию о заметке по её идентификатору.
    /// </summary>
    static void DetailedInf(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Просмотр информации о заметке по ее номеру\". Введи ID (номер) заметки:[/]");
        string nom = Console.ReadLine();
        if (int.TryParse(nom, out int id))
        {
            var zam = zametkaManager.GetById(id);
            if (zam != null)
            {
                AnsiConsole.MarkupLine($"\n[bold underline]Информация о заметке {zam.Id}[/]");
                AnsiConsole.MarkupLine($"[green]Заголовок:[/] {zam.Title}");
                AnsiConsole.MarkupLine($"[green]Содержимое:[/]\n{zam.Words}");
                AnsiConsole.MarkupLine($"[green]Дата создания:[/] {zam.CreateDate:yyyy-MM-dd}");
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
    
    /// <summary>
    /// Добавляет новую заметку
    /// Заголовок и текст запрашиваю у пользователя
    /// ID генерируется
    /// </summary>
    private static void DobZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Добавить заметку (номер генерируется автоматически)\"[/]");
        AnsiConsole.MarkupLine("Напиши заголовок");
        string title = Console.ReadLine();
        AnsiConsole.MarkupLine("Напиши текст данной заметки");
        string words = MnogostrochniyVvod();
        var newZam = new Zametkapolya
        {
            Id = zametkaManager.GiveId(),
            Title = title,
            Words = words,
            CreateDate = DateTime.Now
        };
        zametkaManager.Zametki.Add(newZam);
        AnsiConsole.MarkupLine("[green] Ты добавил новую заметку, поздравляю![/]");
    }
    
    /// <summary>
    /// Удаляет заметку по идентификатору, введенному пользователем.
    /// </summary>
    private static void UdZam(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow] Ты выбрал пункт \"Удаление заметки\"[/]");
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
    
    /// <summary>
    /// Выводит интерактивную таблицу заметок
    /// Пользователь может выбрать действие для таблицы: оставить без изменений, отсортировать или отфильтровать заметки.
    /// После выбора, обновленные данные отображаются в таблице.
    /// </summary>
    static void SozdTabl(ZametkaManager zametkaManager)
    {   
        var tableAction = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("[blue]Выберите действие для таблицы:[/]")
            .AddChoices(new[] { "Без изменений", "Сортировка", "Фильтрация" })
    );
        
        IEnumerable<Zametkapolya> tableData = zametkaManager.Zametki;

        if (tableAction == "Сортировка")
        {
            var sortType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Выберите тип сортировки:[/]")
                    .AddChoices(new[] { "По дате", "По заголовку" })
            );
            if (sortType == "По дате")
            {
                var sortOrder = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[blue]Выберите порядок сортировки даты:[/]")
                        .AddChoices(new[] { "По возрастанию", "По убыванию" })
                );
                bool ascending = sortOrder == "По возрастанию";
                tableData = SortZametka.ByDate(tableData, ascending);
            }
            else if (sortType == "По заголовку")
            {
                tableData = SortZametka.PervBuk(tableData);
            }
        }
        else if (tableAction == "Фильтрация")
        {
            var filterType = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[blue]Выберите тип фильтрации:[/]")
                    .AddChoices(new[] { "По датам", "По ключевому слову", "Комбинированная" })
            );
            if (filterType == "По датам")
            {
                AnsiConsole.MarkupLine("[blue]Введите начальную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }
                AnsiConsole.MarkupLine("[blue]Введите конечную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }
                var dateFilter = new FilterZametka.DateRangeFilter(startDate, endDate);
                tableData = dateFilter.PrimFilt(tableData);
            }
            else if (filterType == "По ключевому слову")
            {
                AnsiConsole.MarkupLine("[blue]Введите ключевое слово для фильтрации:[/]");
                string keyword = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    AnsiConsole.MarkupLine("[red]Ключевое слово не может быть пустым.[/]");
                    return;
                }
                var keywordFilter = new FilterZametka.KluchSlov(keyword);
                tableData = keywordFilter.PrimFilt(tableData);
            }
            else if (filterType == "Комбинированная")
            {
                AnsiConsole.MarkupLine("[blue]Введите начальную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime comboStart))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }
                AnsiConsole.MarkupLine("[blue]Введите конечную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime comboEnd))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }
                AnsiConsole.MarkupLine("[blue]Введите ключевое слово для фильтрации:[/]");
                string comboKeyword = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(comboKeyword))
                {
                    AnsiConsole.MarkupLine("[red]Ключевое слово не может быть пустым.[/]");
                    return;
                }
                var dateFilterCombo = new FilterZametka.DateRangeFilter(comboStart, comboEnd);
                var keywordFilterCombo = new FilterZametka.KluchSlov(comboKeyword);
                var combinedFilter = new FilterZametka.DvFilt(new List<FilterZametka.IEnZametka> { dateFilterCombo, keywordFilterCombo });
                tableData = combinedFilter.PrimFilt(tableData);
            }
        }
        var tabl = new Table()
            .Border(TableBorder.Square)
            .BorderColor(Color.Chartreuse3)
            .AddColumn("Id")
            .AddColumn("Заголовок")
            .AddColumn("Текст")
            .AddColumn("Дата создания");
        foreach (var el in tableData)
        {
            tabl.AddRow(el.Id.ToString(), el.Title, el.Words, el.CreateDate.ToString("yyyy-MM-dd"));
        }

        AnsiConsole.Write(tabl);
    }
    
    /// <summary>
    /// Выполняет сортировку заметок. Есть 2 варианта: 
    /// сортировка по дате (возр/уб) или по заголовкам (алфавитный порядок)
    /// </summary>
    private static void Sort(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[green] Вы выбрали пункт \"Сортировка заметок\"[/]");
        AnsiConsole.MarkupLine("Выберите один из двух вариантов:");
        AnsiConsole.MarkupLine("1 - Сортировка по дате создания (возрастание/убывание)");
        AnsiConsole.MarkupLine("2 - Сортировка по заголовкам");
        bool down = false;
        bool up = true;
        string input = Console.ReadLine();
        switch (input)
        {
            case "1":
                AnsiConsole.MarkupLine("Вы выбрали вариант \"Сортировка по дате создания\"");
                AnsiConsole.MarkupLine("Выберете как будете сортировать: по возрастанию (1) или убывания (2) даты");
                string input2 = Console.ReadLine();
                switch (input2)
                {
                    case "1":
                        var sortDateUp = SortZametka.ByDate(zametkaManager.Zametki, up).ToList();
                        zametkaManager.Zametki = sortDateUp;
                        AnsiConsole.MarkupLine("[green]Заметки отсортированы по дате создания[/]");
                        break;
                    case "2":
                        var sortDateDown = SortZametka.ByDate(zametkaManager.Zametki, down).ToList();
                        zametkaManager.Zametki = sortDateDown;
                        AnsiConsole.MarkupLine("[green]Заметки отсортированы по дате создания[/]");
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red] Неправильный выбор![/]");
                        return;
                }

                break;
            case "2":
                AnsiConsole.MarkupLine("Вы выбрали вариант \"Сортировка по заголовкам\"");
                var sortedByTitle = SortZametka.PervBuk(zametkaManager.Zametki).ToList();
                zametkaManager.Zametki = sortedByTitle;
                AnsiConsole.MarkupLine("[green]Заметки отсортированы по заголовку (алфавитный порядок)[/]");
                break;

        }
    }
    
    /// <summary>
    /// Выполняет фильтрацию заметок. Есть 3 варианта: 
    /// Фильтрация по датам, по ключевому слову или комбинированно
    /// </summary>
    private static void Filtr(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[green] Вы выбрали пункт \"Фильтрация заметок\"[/]");
        AnsiConsole.MarkupLine("Выберите один из трех вариантов:");
        AnsiConsole.MarkupLine("1 - Фильтрация по датам");
        AnsiConsole.MarkupLine("2 - Фильтрация по ключевому слову в заголовке или в тексте");
        AnsiConsole.MarkupLine("3 - Комбинированная фильтрация");
        IEnumerable<Zametkapolya> filterZam = zametkaManager.Zametki;
        string input = Console.ReadLine();
        switch (input)
        {
            case "1":
                AnsiConsole.MarkupLine("[blue]Введите дату старта (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }

                AnsiConsole.MarkupLine("[blue]Введите конечную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime endDate))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }

                var dateFilter = new FilterZametka.DateRangeFilter(startDate, endDate);
                filterZam = dateFilter.PrimFilt(filterZam);
                break;
            case "2":
                AnsiConsole.MarkupLine("[blue]Введите ключевое слово для фильтрации:[/]");
                string keyword = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    AnsiConsole.MarkupLine("[red]Ключевое слово не может быть пустым.[/]");
                    return;
                }

                var keywordFilter = new FilterZametka.KluchSlov(keyword);
                filterZam = keywordFilter.PrimFilt(filterZam);
                break;
            case "3":
                AnsiConsole.MarkupLine("[blue]Введите начальную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime comboStart))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }

                AnsiConsole.MarkupLine("[blue]Введите конечную дату (ГГГГ-ММ-ДД):[/]");
                if (!DateTime.TryParse(Console.ReadLine(), out DateTime comboEnd))
                {
                    AnsiConsole.MarkupLine("[red]Неверный формат даты.[/]");
                    return;
                }

                AnsiConsole.MarkupLine("[blue]Введите ключевое слово для фильтрации:[/]");
                string comboKeyword = Console.ReadLine() ?? "";
                if (string.IsNullOrWhiteSpace(comboKeyword))
                {
                    AnsiConsole.MarkupLine("[red]Ключевое слово не может быть пустым.[/]");
                    return;
                }

                var dateFilterCombo = new FilterZametka.DateRangeFilter(comboStart, comboEnd);
                var keywordFilterCombo = new FilterZametka.KluchSlov(comboKeyword);
                var combinedFilter = new FilterZametka.DvFilt(new List<FilterZametka.IEnZametka>
                    { dateFilterCombo, keywordFilterCombo });
                filterZam = combinedFilter.PrimFilt(filterZam);
                break;
            default:
                AnsiConsole.MarkupLine("[red]Неверный выбор варианта фильтрации[/]");
                return;
        }
    }
    
    /// <summary>
    /// Выполняет редактирование заметки.
    /// Запрашивает у пользователя ID заметки, если такая заметка существует, то просим ввести новый заголовок и новый текст заметки
    /// Если поля остаются пустыми, сохраняются текущие значения.
    /// Ввод текста - многострочный, в конце нужно добавить HSE
    /// </summary>
    private static void Izm(ZametkaManager zametkaManager)
    {
        AnsiConsole.MarkupLine("[yellow]Вы выбрали пункт \"Редактирование заметки\"[/]");
        AnsiConsole.MarkupLine("[blue]Введите ID заметки для редактирования:[/]");
        string? input = Console.ReadLine();
        if (!int.TryParse(input, out int id))
        {
            AnsiConsole.MarkupLine("[red]Некорректный ID![/]");
            return;
        }

        var zam = zametkaManager.GetById(id);
        if (zam == null)
        {
            AnsiConsole.MarkupLine($"[red]Заметка с таким  {id} не найдена![/]");
            return;
        }

        AnsiConsole.MarkupLine($"[blue]Текущий заголовок: {zam.Title}[/]");
        AnsiConsole.MarkupLine("[blue]Введите новый заголовок. Можете оставить пустым, тогда он не иземенится:[/]");
        string newTitle = Console.ReadLine() ?? "";
        if (!string.IsNullOrWhiteSpace(newTitle))
        {
            zam.Title = newTitle;
        }

        AnsiConsole.MarkupLine(
            "[blue]Введите новый текст заметки. Для завершения введите строку [bold]HSE[/]):[/]");
        string newContent = MnogostrochniyVvod();
        if (!string.IsNullOrWhiteSpace(newContent))
        {
            zam.Words = newContent;
        }

        AnsiConsole.MarkupLine($"[green]Заметка с номером {zam.Id} успешно обновлена![/]");
    }
    
    /// <summary>
    /// Метод для многострочного ввода текста заметки
    /// Когда все написали, на новой строке нужно добавить HSE
    /// </summary>
    private static string MnogostrochniyVvod()
    {
        var lines = new List<string>();
        while (true)
        {
            string? line = Console.ReadLine();
            if (line?.Trim().ToUpper() == "HSE")
                break;
            lines.Add(line);
        }

        return string.Join(Environment.NewLine, lines);
    }
    
    /// <summary>
    /// Метод для поиска синонимов
    /// Если введен id заметки, то заменяем  только в выбранной заметке
    /// 0 – во всех заметках
    /// Используется API для получения синонимов, а затем производится замена слова на выбранный синоним.
    /// </summary>
    private static async Task IskSyn(ZametkaManager zametkaManager)
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Введите ID заметки для поиска синонимов или 0 для поиска по слову:[/]");
        string input = Console.ReadLine();
        if (!int.TryParse(input, out int zamId))
        {
            AnsiConsole.MarkupLine("[red]Некорректный ввод![/]");
            return;
        }

        string word;
        if (zamId == 0)
        {
            AnsiConsole.MarkupLine("[blue]Введите слово для поиска синонимов:[/]");
            word = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(word))
            {
                AnsiConsole.MarkupLine("[red]Слово не может быть пустым.[/]");
                return;
            }
        }
        else
        {
            var zam = zametkaManager.GetById(zamId);
            if (zam == null)
            {
                AnsiConsole.MarkupLine("[red]Заметка с таким ID не найдена.[/]");
                return;
            }

            AnsiConsole.MarkupLine("[blue]Введите слово из заметки для поиска синонимов:[/]");
            word = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(word))
            {
                AnsiConsole.MarkupLine("[red]Слово не может быть пустым.[/]");
                return;
            }
        }

        var synonymService = new ApiWork();
        var synonyms = await synonymService.NaitiSinon(word);
        if (synonyms.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Синонимы не найдены.[/]");
            return;
        }
        
        string chosenSynonym = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[blue]Выберите синоним для замены:[/]")
                .AddChoices(synonyms)
        );

        if (zamId != 0)
        {
            var zam = zametkaManager.GetById(zamId);
            if (zam != null)
            {
                zam.Words = Regex.Replace(zam.Words, word, chosenSynonym, RegexOptions.IgnoreCase);
                AnsiConsole.MarkupLine(
                    $"[green]В заметке {zam.Id} слово \"{word}\" заменено на \"{chosenSynonym}\".[/]");
            }
        }
        else
        {   
            foreach (var zam in zametkaManager.Zametki)
            {
                zam.Words = Regex.Replace(zam.Words, word, chosenSynonym, RegexOptions.IgnoreCase);
            }
            AnsiConsole.MarkupLine($"[green]Во всех заметках все вхождения слова \"{word}\" заменены на \"{chosenSynonym}\".[/]");
        }
    }
    
    ///  <summary>
    /// Метод для вывода календаря заметок.
    /// Календарь с января 2015 по декабря текущего (2025)
    /// События по-разному выделяются стилями в зависимости от количества заметок в день
    /// </summary>
    static void SozdCal(ZametkaManager zametkaManager)
    {
        var zametk = zametkaManager.Zametki
            .GroupBy(n => n.CreateDate.Date)
            .ToDictionary(g => g.Key, g => g.Count());

        int startYear = 2015;
        int endYear = DateTime.Now.Year;

        for (int year = startYear; year <= endYear; year++)
        {
            for (int month = 1; month <= 12; month++)
            {
                var cal = new Calendar(year, month)
                    .Culture("ru-RU")
                    .HeaderStyle(Style.Parse("blue bold"));
                
                foreach (var date in zametk.Keys)
                {
                    if (date.Year == year && date.Month == month)
                    {
                        if (zametk[date] == 1)
                        {
                            cal.AddCalendarEvent(date, new Style(Color.Yellow));
                        }
                        else if (zametk[date] > 1)
                        {
                            cal.AddCalendarEvent(date, new Style(Color.Yellow, decoration: Decoration.Bold));
                        }
                    }
                }
                
                AnsiConsole.Write(cal);
                AnsiConsole.WriteLine();
                
            }
        }
    }
    
    /// <summary>
    /// Класс предназначенный для реализации синхранизации заметок в Google Drive
    /// </summary>
    public static void SyncGoogleDrive()
    {
        
        string csvPath = "notes.csv";
        string jsonPath = "notes.json";

        // Автоматически создаем пустые файлы, если их нет
        if (!File.Exists(csvPath))
        {
            File.Create(csvPath).Dispose();
        }
        if (!File.Exists(jsonPath))
        {
            File.Create(jsonPath).Dispose();
        }

        AnsiConsole.MarkupLine("[blue]Выберите формат для синхронизации с Google Drive:[/]");
        AnsiConsole.MarkupLine("1. CSV");
        AnsiConsole.MarkupLine("2. JSON");
        string? choice = Console.ReadLine();

        string fileToSync = (choice == "2") ? jsonPath : csvPath;

        try
        {
            var manager = new CloudManager();
            manager.SyncFile(fileToSync);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red]Ошибка при синхронизации: {ex.Message}[/]");
        }
    }
}