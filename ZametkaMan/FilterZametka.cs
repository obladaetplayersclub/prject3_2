using Zametka;
namespace ZametkaMan;

public class FilterZametka
{
    public interface IEnZametka
        {
            IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zam);
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

            public IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zam)
            {
                List<Zametkapolya> результат = new List<Zametkapolya>();
                foreach (var el in zam)
                {
                    if (el.CreateDate.Date >= StartDate && el.CreateDate.Date <= EndDate)
                    {
                        результат.Add(el);
                    }
                }
                return результат;
            }
        }
        public class KluchSlov : IEnZametka
        {
            public string Kluch { get; }

            public KluchSlov(string keyword)
            {
                Kluch = keyword;
            }

            public IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zam)
            {
                List<Zametkapolya> zametkis = new List<Zametkapolya>();
                foreach (var el in zam)
                {
                    if ((el.Title != null && el.Title.IndexOf(Kluch, StringComparison.OrdinalIgnoreCase) >= 0)
                        || (el.Words != null && el.Words.IndexOf(Kluch, StringComparison.OrdinalIgnoreCase) >= 0))
                    {
                        zametkis.Add(el);
                    }
                }
                return zametkis;
            }
        }
        
        public class CombinedFilter : IEnZametka
        {
            private readonly IEnumerable<IEnZametka> flit;

            public CombinedFilter(IEnumerable<IEnZametka> filters)
            {
                flit = filters;
            }

            public IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zamtk)
            {
                IEnumerable<Zametkapolya> res = zamtk;
                foreach (var el in flit)
                {
                    res = el.PrimFilt(res);
                }
                return res;
            }
        }
}