namespace CitiesManager.Core.ServiceContracts
{
    public interface ICitiesDeleterService
    {
        Task<bool> DeleteCity(Guid? cityID);
    }
}
