using CitiesManager.Core.Domain.Entities;

namespace CitiesManager.Core.DTO
{
    /// <summary>
    /// DTO class that is used as return type for most of CitiesService methods
    /// </summary>
    public class CityResponse
    {
        public Guid CityID { get; set; }
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


        //It compares the current object to another object of CountryResponse type and returns true, if both values are same; otherwise returns false
        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj.GetType() != typeof(CityResponse))
            {
                return false;
            }
            CityResponse city_to_compare = (CityResponse)obj;

            return CityID == city_to_compare.CityID && CityName == city_to_compare.CityName;
        }

        //returns an unique key for the current object
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public static class CityExtensions
    {
        //Converts from City object to ToCityResponse object
        public static CityResponse ToCityResponse(this City city)
        {
            return new CityResponse() { 
                CityID = city.CityID, 
                CityName = city.CityName,
                DateOfFoundation = city.DateOfFoundation,
                CityHistory = city.CityHistory,
                Population = city.Population,
                ZipCode = city.ZipCode,
                Description = city.Description

            };
        }
    }
}
