using CitiesManager.Core.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CitiesManager.Core.DTO
{
    /// <summary>
    /// 30.06.2023. - dodao static klase:CityExtensionsRecord i CityResponseRecord, zarad automatizacije u CitiesCRUDService
    /// </summary>
    public static class CityExtensionsRecord
    {
        public static CityResponseRecord ToCityResponseRecord(this City city)
        {
            return new CityResponseRecord
                (
                    city.CityID,
                    city.CityName,
                    city.DateOfFoundation,
                    city.CityHistory,
                    city.Population,
                    city.ZipCode,
                    city.Description
                );
        }

    }

    /// <summary>
    /// 29.06.2023. 
    /// Milan Jovanović 
    /// Record za DTO
    /// </summary>
    /// <param name="CityID"></param>
    /// <param name="CityName"></param>
    /// <param name="DateOfFoundation"></param>
    /// <param name="CityHistory"></param>
    /// <param name="Population"></param>
    /// <param name="ZipCode"></param>
    /// <param name="Description"></param>
    public record CityResponseRecord(
        Guid CityID,
        string? CityName,
        /// <summary>
        /// Datum osnivanja
        /// </summary>
        DateTime? DateOfFoundation,
        string? CityHistory,
        /// <summary>
        /// broj stanovnika
        /// </summary>
        int Population,
        String? ZipCode,
        /// <summary>
        /// napomene, opis
        /// </summary>
        string? Description
    );

}
