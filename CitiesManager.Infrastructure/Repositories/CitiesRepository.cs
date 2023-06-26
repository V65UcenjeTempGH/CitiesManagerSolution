using Azure;
using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers;
using CitiesManager.WebAPI.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing.Printing;
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
        public async Task<City> AddCity(City city)
        {
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();
            return city;

        }

        public async Task<bool> DeleteCityByCityID(Guid cityID)
        {
            _db.Cities.RemoveRange(_db.Cities.Where(temp => temp.CityID == cityID));
            int rowsDeleted = await _db.SaveChangesAsync();

            return rowsDeleted > 0;
        }

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
        public async Task<PagedList<CityResponse>> GetAllCitiesPg(CityParameters cityParameters)
        {
            _logger.LogInformation("GetAllCities of CitiesRepository Pagination");

            // zamenuo sa PagedList
            //return await _db.Cities.Skip(skip).Take(cityParameters.PageSize).ToListAsync();

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

            // Sort ... Milan Jovanović
            // Izraz F-je koja prihvata city i vraća objekat tj svojstvo po kojem sortiram pod imenom keySelector
            Expression<Func<City, object>> keySelector = cityParameters.SortColumn?.ToLower() switch
            {
                "cityname" => city => city.CityName!,
                "zipcode" => city => city.ZipCode!,
                "population" => city => city.Population,
                _ => city => city.CityID
            };

            if (cityParameters.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(keySelector);
            }
            else
            {
                query = query.OrderBy(keySelector);
            }
            // End Sort

            var cityDTO = query.Select(c => new CityResponse
            {
                CityID = c.CityID,
                CityName = c.CityName,
                CityHistory = c.CityHistory,
                ZipCode = c.ZipCode,
                DateOfFoundation = c.DateOfFoundation,
                Description = c.Description,
                Population = c.Population
            });

            int count = await cityDTO.CountAsync();
            var items = await cityDTO.Skip((cityParameters.PageNumber - 1) * cityParameters.PageSize)
                .Take(cityParameters.PageSize)
                .ToListAsync();

            return new PagedList<CityResponse>(items, count, cityParameters.PageNumber, cityParameters.PageSize);


        }

        /// <summary>
        /// Prebroj ...
        /// ne koristim ga...
        /// </summary>
        /// <returns>Return ukupan broj slogova</returns>
        public async Task<int> GetCountAsync()
        {
            return await _db.Cities.CountAsync();
        }

        public async Task<City?> GetCityByCityID(Guid cityID)
        {
            return await _db.Cities.FirstOrDefaultAsync(temp => temp.CityID == cityID);
        }

        public async Task<List<City>> GetFilteredCities(Expression<Func<City, bool>> predicate)
        {
            _logger.LogInformation("GetFilteredCities of CitiesRepository");

            return await _db.Cities
             .Where(predicate)
             .ToListAsync();
        }

        public async Task<City> UpdateCity(City city)
        {
            City? matchingCity = await _db.Cities.FirstOrDefaultAsync(temp => temp.CityID == city.CityID);
            if (matchingCity == null)
                return city;

            matchingCity.CityName = city.CityName;
            matchingCity.DateOfFoundation = city.DateOfFoundation;
            matchingCity.CityHistory = city.CityHistory;
            matchingCity.ZipCode = city.ZipCode;
            matchingCity.Population = city.Population;
            matchingCity.Description = city.Description;


            int countUpdated = await _db.SaveChangesAsync();

            return matchingCity;
        }


    }
}
