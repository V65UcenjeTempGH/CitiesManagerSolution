using CitiesManager.Core.DTO;

namespace CitiesManager.Core.ServiceContracts
{
    public interface ICitiesAdderService
    {
        Task<CityResponse> AddCity(CityAddRequest? cityAddRequest);
    }
}
