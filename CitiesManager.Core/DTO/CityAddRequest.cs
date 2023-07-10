using CitiesManager.Core.Domain.Entities;

namespace CitiesManager.Core.DTO
{
    /// <summary>
    /// DTO class for adding a new city ... ista napomena kao i za klasu City
    /// </summary>
    public class CityAddRequest
    {
        public string? CityName { get; set; }
        /// <summary>
        /// Datum osnivanja
        /// </summary>
        public DateTime? DateOfFoundation { get; set; }
        public string? CityHistory { get; set; }
        /// <summary>
        /// broj stanovnika
        /// </summary>
        public int Population { get; set; }
        public String? ZipCode { get; set; }
        /// <summary>
        /// napomene, opis
        /// </summary>
        public string? Description { get; set; }


        public City ToCity()
        {
            return new City() { 
                CityName = CityName,
                DateOfFoundation = DateOfFoundation,
                CityHistory = CityHistory,
                Population = Population,
                ZipCode = ZipCode,
                Description = Description
            };
        }
    }
}
