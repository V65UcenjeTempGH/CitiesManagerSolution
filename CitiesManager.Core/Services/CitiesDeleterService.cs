using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.ServiceContracts;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.Services
{
    public class CitiesDeleterService : ICitiesDeleterService
    {
        //private field
        private readonly ICitiesRepository _citiesRepository;
        private readonly ILogger<CitiesGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;


        //constructor
        public CitiesDeleterService(ICitiesRepository citiesRepository, ILogger<CitiesGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _citiesRepository = citiesRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task<bool> DeleteCity(Guid? cityID)
        {
            if (cityID == null)
            {
                throw new ArgumentNullException(nameof(cityID));
            }

            City? city = await _citiesRepository.GetCityByCityID(cityID.Value);
            if (city == null)
                return false;

            await _citiesRepository.DeleteCityByCityID(cityID.Value);

            return true;
        }
    }
}
