using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using FluentValidation;

namespace CitiesManager.Core.Helpers.Validators
{
    /// <summary>
    /// 12.07.2023.
    /// Instalirao package: FluentValidation.DependencyInjectionExtensions ... uninstall
    ///                     FluentValidation.AspNetCore ... ovo ostavio
    /// </summary>
    public sealed class CityAddValidator : AbstractValidator<CityAddRequest>
    {

        //private readonly ICitiesRepository _citiesRepository;

        public CityAddValidator(ICitiesRepository citiesRepository) 
        {
            //_citiesRepository = citiesRepository;

            RuleFor(c => c.ZipCode)
                .MustAsync(async (zipcode, _) =>
                    {
                       return await citiesRepository.IsZipCodeUniqueAsync(zipcode);
                    }).WithMessage("ZipCode must be unique.")
                .NotEmpty().WithMessage("ZipCode is required.")
                .Length(5).WithMessage("ZipCode must have 5 char.");


            RuleFor(c => c.CityName)
                .MustAsync(async (cityname, _) =>
                {
                    return await citiesRepository.IsCityNameUniqueAsync(cityname);
                }).WithMessage("City name must be unique.")
                .NotEmpty().WithMessage("City name is required.")
                .MaximumLength(50).WithMessage("City name must have 5 char.");

            RuleFor(c => c.Population)
                .NotEmpty().WithMessage("City name is required.")
                .InclusiveBetween(0,50000000).WithMessage("Broj stanivnka mora biti u opsegu 0 i 50M ")
                .GreaterThanOrEqualTo(0).WithMessage("Broj stanivnka mora biti >=0 ");
            
        }


    }
}
