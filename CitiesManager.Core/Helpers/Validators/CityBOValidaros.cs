using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CitiesManager.Core.Helpers.Validators
{
    public  class CityBOValidaros :ICityBOValidaros
    {
        private readonly ICitiesRepository _citiesRepository;

        public string[]? Errors { get; set; }
        public CityBOValidaros(ICitiesRepository citiesRepository) 
        {
            _citiesRepository = citiesRepository;
        }

        public async Task<bool> IsZipCodeCityName4UpdateAsyncUnique(string? zipCode, string?cityName)
        {
            // akko je CityName ili ZipCode prazan - ne testiraj, dočekaće ga druga validacija, ali tada bez cimanja BP
            if ((string.IsNullOrWhiteSpace(cityName)) || (string.IsNullOrWhiteSpace(zipCode)))
                return true;

            // validation logic
            (bool llOkZipCode, bool llOkCytyName) = (true, true);
            llOkZipCode = await _citiesRepository.IsZipCodeCitynameUnique4UpdateAsync(zipCode, cityName);
            if (!llOkZipCode) 
            {
                AddError($"{zipCode} mora biti jedinstven");
            }
            else
            {
                llOkCytyName = llOkZipCode && await _citiesRepository.IsCityNameZipCodeUnique4UpdateAsync(cityName, zipCode);
                if (!llOkCytyName)
                {
                    AddError($"{cityName} mora biti jedinstven");
                }
            }

            bool llOk = llOkZipCode && llOkCytyName;
            return llOk;
        }

        public void AddError(string error)
        {
            if (Errors == null)
            {
                Errors = new string[] { error };
            }
            else
            {
                var tempList = Errors.ToList();
                tempList.Add(error);
                Errors = tempList.ToArray();
            }
        }

        public async Task<bool> IsCityUpdateValid(CityUpdateRequest? cityUpdateRequest)
        {

           bool llOk = await IsZipCodeCityName4UpdateAsyncUnique(cityUpdateRequest?.ZipCode, cityUpdateRequest?.CityName);

            return llOk;
        }

    }
}
