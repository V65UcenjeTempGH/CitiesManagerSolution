namespace CitiesManager.Core.Helpers
{
    /// <summary>
    /// 24.06.2023. - za ovaj slučaj, morao sam da instaliram NPM Microsoft.EntityFrameworkCore, koji sam posle i "skinuo", jer sam rešio na drugi način. Pogledaj komentar "VERZIJA_3" 
    /// Pozivaju ga: 
    /// - Controller (CityController), 
    /// - CitiesServiceCRUD, ICitiesServiceCRUD, 
    /// - CitiesRepository, ICitiesRepository
    /// </summary>
    public class PagedList<T> : List<T>
    {

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; set; }

        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;

            AddRange(items);
        }


        /// <summary>
        /// Razne izmene/verzije/korekcije: 25.06.2023. 20:46
        /// 
        /// VERZIJA_1 - public static async Task<PagedList<T>> ToPagedList(...)
        /// ovo svakako radi, polazna verzija: 
        /// 
        //public static async Task<PagedList<T>> ToPagedList(IQueryable<T> source, int pageNumber, int pageSize)
        //{
        //    int count = await source.CountAsync();
        //    var items = await source.Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();
        //    return new PagedList<T>(items, count, pageNumber, pageSize);
        //}
        ///
        /// VERZIJA_2 - public static PagedList<T> CreatePagedList(...)
        /// Pokušaj da iz Repository pokupim i spremim podatke i kao takve prosledim metodi 
        /// public static PagedList<T> CreatePagedList(List<T> items, int count, int pageNumber, int pageSize)
        /// 
        /// </summary>
        /// <param name="items"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        //public static PagedList<T> CreatePagedList(List<T> items, int count, int pageNumber, int pageSize)
        //{
        //    return new PagedList<T>(items, count, pageNumber, pageSize);
        //}
        ///
        /// 
        /// VERZIJA_3:
        /// Pokušaj kao iz slučaja VERZIJA_2 (pokupim i spremim podatke) ali ne šaljem ih metodi 
        /// public static PagedList<T> CreatePagedList(List<T> items, int count, int pageNumber, int pageSize) već direktno iz Repository pozivam return new PagedList<T>(items, count, pageNumber, pageSize) tj konstruktor klase PagedList()
        /// time sam hteo da prebacim pristup podacima iz DB iz Core u Infrastructure i tim pristupom u Core ne bi trebao da imam instaliran iz NPM Microsoft.EntityFrameworkCore
    }
}
