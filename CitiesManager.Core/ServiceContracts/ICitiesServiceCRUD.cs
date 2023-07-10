using CitiesManager.Core.DTO;
using CitiesManager.Core.Helpers;

namespace CitiesManager.Core.ServiceContracts
{
    /// <summary>
    /// Ranije, do 03.07.2023. bila su 4 servisa, za svaku CRUD operaciju po jedan
    /// sada je kompletan CRUD (plus još CSV, Xlsx) u ovaj jedan servis
    ///  Napomena: 
    /// do ovog primera za PK kada je Oracle ili PostGreSQL u pitanju, koristio sam sequence (desktop app, nTier)
    /// </summary>
    public interface ICitiesServiceCRUD
    {

        /// <summary>
        /// </summary>
        /// <returns>CityResponseRecord</returns>
        Task<List<CityResponseRecord>> GetAllCitiesRc();

        /// <summary>
        /// 23.06.2023. - PagedList
        /// </summary>
        /// <param name="cityParameters"></param>
        /// <returns></returns>
        Task<PagedList<CityResponseRecord>> GetAllCitiesPg(CityParameters cityParameters);

        /// <summary>
        /// Returns the city object based on the given city id
        /// </summary>
        /// <param name="cityID">City id to search</param>
        /// <returns>Returns matching city object</returns>
        Task<CityResponseRecord?> GetCityByCityID(Guid? cityID);

        /// <summary>
        /// NE KORISTIM GA, ali neka ostane kao primer 
        /// Umesto njega koristim GetAllCitiesPg(CityParameters cityParameters)
        /// Returns all city objects that matches with the given search field and search string
        /// </summary>
        /// <param name="searchBy">Search field to search</param>
        /// <param name="searchString">Search string to search</param>
        /// <returns>Returns all matching cities based on the given search field and search string</returns>
        Task<List<CityResponseRecord>> GetFilteredCities(string searchBy, string? searchString);


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

        /// <summary>
        /// 03.07.2023. - Uklonio ICityDeleteService
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        Task<bool> DeleteCity(Guid? cityID);

        /// <summary>
        /// 03.07.2023. - Ukolnio ICitiesAdderService
        /// </summary>
        /// <param name="cityAddRequest"></param>
        /// <returns></returns>
        Task<CityResponseRecord> AddCity(CityAddRequest? cityAddRequest);

        /// <summary>
        /// 03.07.2023. - Ukolnio ICitiesUpdaterService
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cityUpdateRequest"></param>
        /// <returns>CityResponseRecord</returns>
        Task<CityResponseRecord?> UpdateCity(Guid id, CityUpdateRequest? cityUpdateRequest);

    }
}
