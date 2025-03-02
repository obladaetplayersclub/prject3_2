using Zametka;
namespace ZametkaMan;

/// <summary>
/// Класс, содержащий различные реализации фильтрации заметок
/// </summary>
public class FilterZametka
{   
    /// <summary>
    /// Интерфейс для фильтра
    /// </summary>
    public interface IEnZametka
    {
        IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zam);
    }
    
    /// <summary>
    /// Класс для фильттрации заметок по датам (диапазон)
    /// 
    /// </summary>
    public class DateRangeFilter : IEnZametka
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public DateRangeFilter(DateTime startDate, DateTime endDate)
        {
            StartDate = startDate.Date;
            EndDate = endDate.Date;
        }
        
        /// <summary>
        /// Применение фильтра по диапазону дат к заметкам
        /// </summary>
        public IEnumerable<Zametkapolya> PrimFilt(IEnumerable<Zametkapolya> zam)
        {
            List<Zametkapolya> res = new List<Zametkapolya>();
            foreach (var el in zam)
            {
                if (el.CreateDate.Date >= StartDate && el.CreateDate.Date <= EndDate)
                {
                    res.Add(el);
                }
            }
            return res;
        }
    }
    
    /// <summary>
    /// Фильтрация по ключ слову: ищет либо в заголовке, либо в содержании
    /// </summary>
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
    
    /// <summary>
    /// Комбинированный фильтр для заметок (по датам + ключевому слову)
    /// </summary>
    public class DvFilt : IEnZametka
    {
        private readonly IEnumerable<IEnZametka> flit;

        public DvFilt(IEnumerable<IEnZametka> filters)
        {
            flit = filters;
        }
        
        /// <summary>
        /// Применяет все заданные фильтры к заметкам
        /// </summary>
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