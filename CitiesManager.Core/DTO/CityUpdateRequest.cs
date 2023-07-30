using CitiesManager.Core.Domain.Entities;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.DTO
{
    /// <summary>
    /// Validation has been moved to:
    /// CityUpdateValidator and CityAddValidator
    /// Note: 
    /// this is only an example and does not correspond to a real data structure
    /// </summary>
    public class CityUpdateRequest
    {
        //[Required(ErrorMessage = "City ID can't be blank")]
        public Guid CityID { get; set; }

        //[Required(ErrorMessage = "City Name can't be blank")]
        public string? CityName { get; set; }

        public DateTime? DateOfFoundation { get; set; }
        public string? CityHistory { get; set; }
        /// <summary>
        /// broj stanovnika grada
        /// </summary>
        public int Population { get; set; }

        //[Required(ErrorMessage = "ZipCode can't be blank")]
        public string? ZipCode { get; set; }
        /// <summary>
        /// napomene, opis
        /// </summary>
        public string? Description { get; set; }


        /// <summary>
        /// Converts the current object of CityAddRequest into a new object of City type
        /// </summary>
        /// <returns>Returns City object</returns>
        public City ToCity()
        {
            return new City()
            {
                CityID = CityID,  
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
