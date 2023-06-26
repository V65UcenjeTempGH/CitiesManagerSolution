using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
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
    public class CitiesUpdaterService : ICitiesUpdaterService
    {
        //private field
        private readonly ICitiesRepository _citiesRepository;
        private readonly ILogger<CitiesGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;


        //constructor
        public CitiesUpdaterService(ICitiesRepository citiesRepository, ILogger<CitiesGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _citiesRepository = citiesRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public async Task<CityResponse> UpdateCity(CityUpdateRequest? cityUpdateRequest)
        {
            if (cityUpdateRequest == null)
                throw new ArgumentNullException(nameof(cityUpdateRequest));

            //validation
            //ValidationHelper.ModelValidation(cityUpdateRequest);

            //get matching city object to update
            City? matchingCity = await _citiesRepository.GetCityByCityID(cityUpdateRequest.CityID);
            if (matchingCity == null)
            {
                //throw new InvalidCityIDException("Given city id doesn't exist");
            }

            //update all details
            matchingCity!.CityName = cityUpdateRequest.CityName;

            await _citiesRepository.UpdateCity(matchingCity); //UPDATE

            return matchingCity.ToCityResponse();
        }
    }
}
