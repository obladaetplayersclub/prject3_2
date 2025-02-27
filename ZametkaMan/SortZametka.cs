namespace Zametka;

public static class SortZametka
{
    public static IEnumerable<Zametkapolya> ByDate(IEnumerable<Zametkapolya> zametki, bool upDownDate = false)
    {
        return upDownDate ? zametki.OrderBy(n => n.CreateDate) : zametki.OrderByDescending(n => n.CreateDate);
    }

    public static IEnumerable<Zametkapolya> ByLetter(IEnumerable<Zametkapolya> zametki)
    {
        return zametki.OrderBy(n => n.Title, StringComparer.OrdinalIgnoreCase);
    }
}