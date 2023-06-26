using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers;
using System.Linq.Expressions;


namespace CitiesManager.Core.Domain.RepositoryContracts
{
    /// <summary>
    /// Represents data access logic for managing City entity
    /// </summary>
    public interface ICitiesRepository
    {
        /// <summary>
        /// Adds a city object to the data store
        /// </summary>
        /// <param name="city">City object to add</param>
        /// <returns>Returns the city object after adding it to the table</returns>
        Task<City> AddCity(City city);


        /// <summary>
        /// Returns all Cities in the data store
        /// </summary>
        /// <returns>List of City objects from table</returns>
        Task<List<City>> GetAllCities();

        /// <summary>
        /// 22.06.2023.
        /// 23.06.2023. zamenuo: pageNumber, pageSize sa CityParameters cityParameters
        /// 23.06.2023. - PagedList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedList<CityResponse>> GetAllCitiesPg(CityParameters cityParameters);

        /// <summary>
        /// 23.06.2023.
        /// </summary>
        /// <returns>return uk.br. slogova</returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// Returns a city object based on the given city id
        /// </summary>
        /// <param name="cityID">CityID (guid) to search</param>
        /// <returns>A city object or null</returns>
        Task<City?> GetCityByCityID(Guid cityID);


        /// <summary>
        /// Returns all city objects based on the given expression
        /// </summary>
        /// <param name="predicate">LINQ expression to check</param>
        /// <returns>All matching cities with given condition</returns>
        Task<List<City>> GetFilteredCities(Expression<Func<City, bool>> predicate);


        /// <summary>
        /// Deletes a city object based on the city id
        /// </summary>
        /// <param name="cityID">City ID (guid) to search</param>
        /// <returns>Returns true, if the deletion is successful; otherwise false</returns>
        Task<bool> DeleteCityByCityID(Guid cityID);


        /// <summary>
        /// Updates a city object (city name and other details) based on the given city id
        /// </summary>
        /// <param name="city">City object to update</param>
        /// <returns>Returns the updated city object</returns>
        Task<City> UpdateCity(City city);
    }
}
