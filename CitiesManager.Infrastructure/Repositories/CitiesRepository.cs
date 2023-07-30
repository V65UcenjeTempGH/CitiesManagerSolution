using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers;
using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace CitiesManager.Infrastructure.Repositories
{
    /// <summary>
    /// Poziva ga Servis
    /// </summary>
    public class CitiesRepository : ICitiesRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<CitiesRepository> _logger;

        public CitiesRepository(ApplicationDbContext db, ILogger<CitiesRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public async Task<List<City>> GetAllCities()
        {
            _logger.LogInformation("GetAllCities of CitiesRepository");

            return await _db.Cities.ToListAsync();
        }

        /// <summary>
        /// 23.06.2023. - Korak 1 - zamenuo: pageNumber, pageSize sa CityParameters 
        /// 23.06.2023. - Korak 2 - PagedList
        /// 25.06.2023. - Korak_4 - Pagination + Filters + Sorting
        /// </summary>
        /// <returns></returns>
        public async Task<PagedList<CityResponseRecord>> GetAllCitiesPg(CityParameters cityParameters)
        {
            _logger.LogInformation("GetAllCities of CitiesRepository Pagination");

            var query = _db.Cities.AsQueryable();

            if (!String.IsNullOrWhiteSpace(cityParameters.CityName))
            {
                query = query.Where(c => c.CityName!.ToLower().Trim().Contains(cityParameters.CityName.Trim().ToLower()));
            }

            if (!String.IsNullOrWhiteSpace(cityParameters.ZipCode))
            {
                query = query.Where(c => c.ZipCode!.ToLower().Trim().Contains(cityParameters.ZipCode.Trim().ToLower()));
            }

            int minPopulation = cityParameters.MinPopulation;
            int maxPopulation = cityParameters.MaxPopulation;

            if (maxPopulation > 0 && minPopulation <= maxPopulation)
            {
                query = query.Where(c => c.Population >= minPopulation && c.Population <= maxPopulation);
            }

            // 09.07.2023. - komentovao, tj ovako je bilo do 09.07.2023.
            // Sort ... 
            // Izraz F-je koja prihvata city i vraća objekat tj svojstvo po kojem sortiram pod imenom keySelector
            //Expression<Func<City, object>> keySelector = cityParameters.SortColumn?.ToLower() switch
            //{
            //    "cityname" => city => city.CityName!,
            //    "zipcode" => city => city.ZipCode!,
            //    "population" => city => city.Population,
            //    _ => city => city.CityID
            //};

            // 09.07.2023. - Dictionary
            // Ovo je dobro, ali može bolje
            //
            //Dictionary<string, Expression<Func<City, object>>> keySelectors = new Dictionary<string, Expression<Func<City, object>>>
            //{
            //    { "cityname", city => city.CityName! },
            //    { "zipcode", city => city.ZipCode! },
            //    { "population", city => city.Population },
            //    { "default", city => city.CityID }
            //};

            //Expression<Func<City, object>> keySelector;
            //if (!keySelectors.TryGetValue(cityParameters.SortColumn?.ToLower(), out keySelector))
            //{
            //    keySelector = keySelectors["default"];
            //}


            // 10.07.2023. 
            // po primedbi/savetu prebaciti Dictionary u static polje 
            // Pogledaj: CitiesManager.Core.Helpers - public static class CityKeySelectors
            //
            //Expression<Func<City, object>> keySelector;
            //if (!CityKeySelectors.KeySelectors.TryGetValue(cityParameters.SortColumn?.ToLower(), out keySelector))
            //{
            //    keySelector = CityKeySelectors.KeySelectors["default"];
            //}

            // ili na ovaj način... a možda može i još bolje ???

            if (!CityKeySelectors.KeySelectors.TryGetValue(cityParameters.SortColumn?.ToLower(), out Expression<Func<City, object>> keySelector))
            {
                keySelector = CityKeySelectors.KeySelectors["default"];
            }

            if (cityParameters.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(keySelector);
            }
            else
            {
                query = query.OrderBy(keySelector);
            }
            // End Sort

            // 03.07.2023. - prebacio na CityResponseRecord
            var cityDTO = query.Select(c => new CityResponseRecord
            (
                 c.CityID,
                 c.CityName,
                 c.DateOfFoundation,
                 c.CityHistory,
                 c.Population,
                 c.ZipCode,
                 c.Description
            ));


            int count = await cityDTO.CountAsync();
            var items = await cityDTO.Skip((cityParameters.PageNumber - 1) * cityParameters.PageSize)
                .Take(cityParameters.PageSize)
                .ToListAsync();

            return new PagedList<CityResponseRecord>(items, count, cityParameters.PageNumber, cityParameters.PageSize);


        }

        public async Task<City?> GetCityByCityID(Guid cityID)
        {
            return await _db.Cities.FirstOrDefaultAsync(temp => temp.CityID == cityID);
        }

        /// <summary>
        /// Prebroj ... ne koristim ga, što ne znači da neću ubuduće ...
        /// </summary>
        /// <returns>Return ukupan broj slogova</returns>
        public async Task<int> GetCountAsync()
        {
            return await _db.Cities.CountAsync();
        }

        /// <summary>
        /// NE KORISTIM GA, ali neka ostane kao primer 
        /// Umesto njega koristim GetAllCitiesPg(CityParameters cityParameters)
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<List<City>> GetFilteredCities(Expression<Func<City, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredCities of CitiesRepository");

            return await _db.Cities
             .Where(predicate)
             .ToListAsync();
        }


        public async Task<City> AddCity(City city)
        {
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();
            return city;

        }

        /// <summary>
        /// Najsporniji deo, počev od Controllera,preko Servisa i samog Repository
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        /// 11.07.2023.  
        public async Task<City?> UpdateCity(City city)
        {

            // 11.07.2023. 
            int countUpdated = await _db
                .Cities
                .Where(c => c.CityID == city.CityID)
                .ExecuteUpdateAsync(c => c
                        .SetProperty(c => c.CityName, city.CityName)
                        .SetProperty(c => c.ZipCode, city.ZipCode)
                        .SetProperty(c => c.DateOfFoundation, city.DateOfFoundation)
                        .SetProperty(c => c.CityHistory, city.CityHistory)
                        .SetProperty(c => c.Description, city.Description)
                        .SetProperty(c => c.Population, city.Population)
                        );

            return (countUpdated > 0) ? city : null;

        }

        /// <summary>
        /// 11.07.2023. 
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        public async Task<bool> DeleteCityByCityID(Guid cityID)
        {

            int rowsDeleted = await _db.Cities
                .Where(c => c.CityID == cityID)
                .ExecuteDeleteAsync();

            return rowsDeleted > 0;

        }

        /// <summary>
        /// 12.07.2023.
        /// Add
        /// Validacija da li je cityname uniq
        /// to može da plus iskontroliše i uniq index
        /// </summary>
        /// <param name="cityname"></param>
        /// <returns></returns>
        public async Task<bool> IsCityNameUniqueAsync(string cityname)
        {
           var lIsUniq = !await _db.Cities.AnyAsync(c => c.CityName == cityname);
           return lIsUniq;
        }

        /// <summary>
        /// 12.07.2023.
        /// Add
        /// Validacija da li je ZipCode uniq
        /// to može da plus iskontroliše i uniq index
        /// </summary>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsZipCodeUniqueAsync(string zipcode)
        {
            return !await _db.Cities.AnyAsync(c => c.ZipCode == zipcode);
            //return lIsUniq;
        }


        /// <summary>
        /// 12.07.2023.
        /// Update
        /// Ne može isti zipcode za drugo cytyname, tj. ne mogu dva grada da imaju isti zipcode
        /// </summary>
        /// <param name="zipcode"></param>
        /// <param name="cityname"></param>
        /// <returns></returns>
        public async Task<bool> IsZipCodeCitynameUnique4UpdateAsync(string? zipcode, string? cityname)
        {
            var lIsUniq = !await _db.Cities.AnyAsync(c => c.ZipCode == zipcode && c.CityName != cityname);
            return lIsUniq;
        }

        /// <summary>
        /// 13.07.2023.
        /// Update
        /// Ne može isti Cityname za drugi ZipCode, tj. ne mogu dva grada (dva različita ZipCoda) da imaju isti naziv
        /// </summary>
        /// <param name="cityname"></param>
        /// <param name="zipcode"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> IsCityNameZipCodeUnique4UpdateAsync(string? cityname, string? zipcode)
        {
            var lIsUniq = !await _db.Cities.AnyAsync(c => c.CityName == cityname && c.ZipCode != zipcode);
            return lIsUniq;
        }
    }
}
