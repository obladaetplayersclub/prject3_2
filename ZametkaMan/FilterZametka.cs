using Zametka;
namespace ZametkaMan;

public class FilterZametka
{
    public interface IEnZametka
    {
        IEnumerable<Zametkapolya> ApplyFilter(IEnumerable<Zametkapolya> notes);
    }
    
    public class DateRangeFilter : IEnZametka
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRangeFilter(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }

        public IEnumerable<Zametkapolya> ApplyFilter(IEnumerable<Zametkapolya> notes)
        {
            return notes.Where(n => n.CreateDate.Date >= StartDate && n.CreateDate.Date <= EndDate);
        }
    }

    public class KeywordFilter : IEnZametka
    {
        public string Keyword { get; }

        public KeywordFilter(string keyword)
        {
            Keyword = keyword;
        }

        public IEnumerable<Zametkapolya> ApplyFilter(IEnumerable<Zametkapolya> notes)
        {
            return notes.Where(n => n.Title.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0
                                    || n.Words.IndexOf(Keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }
    }

    public class CombinedFilter : IEnZametka
    {
        private readonly IEnumerable<IEnZametka> zametkas;

        public CombinedFilter(IEnumerable<IEnZametka> filters)
        {
            zametkas = filters;
        }

        public IEnumerable<Zametkapolya> ApplyFilter(IEnumerable<Zametkapolya> zametki)
        {
            foreach (var zam in zametkas)
            {
                zametki = zam.ApplyFilter(zametki);
            }
            return zametki;
        }
    }
}