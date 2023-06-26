using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.ServiceContracts
{
    public interface ICitiesGetterService
    {
        /// <summary>
        /// Returns all cities
        /// </summary>
        /// <returns>Returns a list of objects of CitiesResponse type</returns>
        Task<List<CityResponse>> GetAllCities();

        /// <summary>
        /// 22.06.2023. Korak 1 - Test Pagination
        /// 23.06.2023. Korak 2 - zamenuo: pageNumber, pageSize sa CityParameters 
        /// 23.06.2023. Korak 3 - PagedList
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<PagedList<CityResponse>> GetAllCitiesPg(CityParameters cityParameters);

        /// <summary>
        /// Returns the city object based on the given city id
        /// </summary>
        /// <param name="cityID">City id to search</param>
        /// <returns>Returns matching city object</returns>
        Task<CityResponse?> GetCityByCityID(Guid? cityID);

        /// <summary>
        /// Returns all city objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching cities based on the given search field and search string</returns>
        Task<List<CityResponse>> GetFilteredCities(string searchBy, string? searchString);


        /// <summary>
        /// Returns cities as CSV
        /// </summary>
        /// <returns>Returns the memory stream with CSV data of cities</returns>
        Task<MemoryStream> GetCitiesCSV();


        /// <summary>
        /// Returns cities as Excel
        /// </summary>
        /// <returns>Returns the memory stream with Excel data of cities</returns>
        Task<MemoryStream> GetCitiesExcel();
    }
}
