namespace Zametka;

/// <summary>
/// Класс предоставляющий сортивроку заметок по датам создания (возр/уб) и заголовку (алф порядок)
/// </summary>
public static class SortZametka
{   
    
    /// <summary>
    /// Сравнение сначала по году, затем по месяцу, в конечном случае по дню
    /// </summary>
    public class ZamDateComparer : IComparer<Zametkapolya>
    {   
        /// <summary>
        /// Сравнивает две заметки по дате создания
        /// </summary>
        /// <param name="x">Первая заметка</param>
        /// <param name="y">Вторая заметка</param>
        /// <returns>
        /// Возвращает 0, если даты создания обеих заметок равны; отрицательное число, если дата создания <paramref name="x"/> меньше,
        /// чем дата создания <paramref name="y"/>; положительное число, если дата создания <paramref name="x"/> больше, чем дата создания <paramref name="y"/>.
        /// </returns>
        public int Compare(Zametkapolya? x, Zametkapolya? y)
        {
            if (x == null && y == null) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            
            int result = x.CreateDate.Year.CompareTo(y.CreateDate.Year);
            if (result != 0)
                return result;
            
            result = x.CreateDate.Month.CompareTo(y.CreateDate.Month);
            if (result != 0)
                return result;
            
            return x.CreateDate.Day.CompareTo(y.CreateDate.Day);
        }
    }
    
    /// <summary>
    /// Сортирует заметки по дате создания
    /// Параметр upDownDate - параметр, отвечающий за то как будем сортировать даты создания:
    /// true - по возрастанию
    /// false - по убыванию
    /// </summary>
    public static IEnumerable<Zametkapolya> ByDate(IEnumerable<Zametkapolya> zametki, bool upDownDate = false /* false- по убыванию, true - по возрастанию */)
    {
        var comparer = new ZamDateComparer();
        return upDownDate 
            ? zametki.OrderBy(n => n, comparer) 
            : zametki.OrderByDescending(n => n, comparer);
    }
    
    /// <summary>
    /// Сортировка заметок по заголовку в алфавитном порядке
    /// </summary>
    public static List<Zametkapolya> PervBuk(IEnumerable<Zametkapolya> zametki)
    {
        List<Zametkapolya> list = new List<Zametkapolya>(zametki);
        
        for (int i = 0; i < list.Count - 1; i++)
        {
            int minIndex = i;
            for (int j = i + 1; j < list.Count; j++)
            {
                if (string.Compare(list[j].Title, list[minIndex].Title, StringComparison.CurrentCultureIgnoreCase) < 0)
                {
                    minIndex = j;
                }
            }
            if (minIndex != i)
            {
                Zametkapolya temp = list[i];
                list[i] = list[minIndex];
                list[minIndex] = temp;
            }
        }
        return list;
    }
}