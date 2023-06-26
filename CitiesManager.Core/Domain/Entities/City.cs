using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Domain.Entities
{
    public class City
    {
        [Key]
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

    }
}
