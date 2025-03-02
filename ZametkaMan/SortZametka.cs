namespace Zametka;

public static class SortZametka
{   
    public class ZamDateComparer : IComparer<Zametkapolya>
    {
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
    
    public static IEnumerable<Zametkapolya> ByDate(IEnumerable<Zametkapolya> zametki, bool upDownDate = false /* false- по убыванию, true - по возрастанию */)
    {
        var comparer = new ZamDateComparer();
        return upDownDate 
            ? zametki.OrderBy(n => n, comparer) 
            : zametki.OrderByDescending(n => n, comparer);
    }

    public static List<Zametkapolya> ByLetter(IEnumerable<Zametkapolya> zametki)
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