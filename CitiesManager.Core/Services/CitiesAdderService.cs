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
    public class CitiesAdderService : ICitiesAdderService
    {
        //private field
        private readonly ICitiesRepository _citiesRepository;
        private readonly ILogger<CitiesGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public CitiesAdderService(ICitiesRepository citiesRepository, ILogger<CitiesGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _citiesRepository = citiesRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        public async Task<CityResponse> AddCity(CityAddRequest? cityAddRequest)
        {
            //check if CityAddRequest is not null
            if (cityAddRequest == null)
            {
                throw new ArgumentNullException(nameof(cityAddRequest));
            }

            //Model validation
            //ValidationHelper.ModelValidation(cityAddRequest);

            //convert cityAddRequest into City type
            City city = cityAddRequest.ToCity();

            //generate CityID
           city.CityID = Guid.NewGuid();

            //add city object to cities list
            await _citiesRepository.AddCity(city);

            //convert the City object into CityResponse type
            return city.ToCityResponse();
        }
    }
}
