using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using FluentValidation;

namespace CitiesManager.Core.Helpers.Validators
{
    /// <summary>
    /// 12.07.2023.
    /// Instalirao package: FluentValidation.DependencyInjectionExtensions
    ///                     FluentValidation.AspNetCore
    /// </summary>
    public sealed class CityUpdateValidator : AbstractValidator<CityUpdateRequest>
    {
        private readonly ICitiesRepository _citiesRepository;
        public CityUpdateValidator(ICitiesRepository citiesRepository)
        {
            _citiesRepository = citiesRepository;

            // Ova treba da spreči da se desi kod Update:
            // - Isti ZipCode za različite imena gradova
            // - Isto ime grada za raličite ZipCod-ove
            // to znači da Sql mora da ima dva parametra kod upita: ZipCode i CityName
            RuleFor(c => new { c.ZipCode, c.CityName })
                 .MustAsync(async (data, _) =>
                 {
                     string? zipcode = data.ZipCode;
                     string? cityname = data.CityName;

                     //akko je CityName ili ZipCode prazan-ne testiraj, dočekaće ga druga validacija, ali tada bez cimanja BP
                     if ((string.IsNullOrWhiteSpace(cityname)) || (string.IsNullOrWhiteSpace(zipcode)))
                         return true;

                     // validation logic
                     bool llOkZipCode = await _citiesRepository.IsZipCodeCitynameUnique4UpdateAsync(zipcode, cityname);
                     bool llOkCytyName = llOkZipCode && await _citiesRepository.IsCityNameZipCodeUnique4UpdateAsync(cityname, zipcode);

                     bool llOk = llOkZipCode && llOkCytyName;
                     return llOk;

                 })
            .WithMessage("{PropertyName} ZipCode/CityName must be unique");
            // pitanje: kako da sam prepozna i napiše ProprtyName u WithMessage  ???

            RuleFor(c => c.CityName)
                .NotEmpty().WithMessage("City name is required.")
                .MaximumLength(50).WithMessage("City name must have 50 char.");

            RuleFor(c => c.ZipCode)
                .NotEmpty().WithMessage("ZipCode is required.")
                .Length(5).WithMessage("ZipCode must have 5 char.");

            // ima viška, ali samo kao primer kako se neki uslovi mogu proveravati - za nauk ! 
            RuleFor(c => c.Population)
                .NotEmpty().WithMessage("City name is required.")
                .InclusiveBetween(0, 50000000).WithMessage("{PropertyName} Broj stanivnka mora biti u opsegu 0 i 50M ")
                .GreaterThanOrEqualTo(0).WithMessage("{PropertyName} Broj stanivnka mora biti >=0 ");

        }


    }
}
