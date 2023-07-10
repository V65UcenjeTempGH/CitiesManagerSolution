using CitiesManager.Core.Domain.Entities;
using System.Linq.Expressions;

namespace CitiesManager.Core.Helpers
{
    /// <summary>
    /// 10.07.2023. po primedbi Milana J.
    /// Pogledaj CitiesRepository ...
    /// </summary>
    public static class CityKeySelectors
    {
        public static Dictionary<string, Expression<Func<City, object>>> KeySelectors = new Dictionary<string, Expression<Func<City, object>>>
        {
            { "cityname", city => city.CityName! },
            { "zipcode", city => city.ZipCode! },
            { "population", city => city.Population },
            { "default", city => city.CityID }
        };
    }
}
