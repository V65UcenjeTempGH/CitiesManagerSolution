using System.ComponentModel.DataAnnotations;

namespace CitiesManager.Core.Domain.Entities
{
    /// <summary>
    /// U celoj priči tj Solution, nije bitna strktura tabele Cities tj ovog City modela
    /// znam da sama struktura ne odgovara realnosti, ovo je bio samo jedan primer kako da primenim pagination+Filter+Sort i najjednostavniji CRUD u Clean Arch.
    /// </summary>
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
