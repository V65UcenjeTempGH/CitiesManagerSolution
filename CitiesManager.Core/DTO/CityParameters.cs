using CitiesManager.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.DTO
{
    /// <summary>
    /// 25.06.2023. - Pozivaju ga: Controlleri, Servisi, IServisi, Repository, IRepository 
    /// </summary>
    public class CityParameters : PaginationParams
    {
        public string? CityName { get; set; }
        public string? ZipCode { get; set; }
        public int MinPopulation { get; set; }  //  = 1000;          // 1K
        public int MaxPopulation { get; set; } //   = 10000000;      // 10M

        /// <summary>
        /// </summary>
        public string? SortColumn { get; set; }
        /// <summary>
        /// </summary>
        public string? SortOrder { get; set; }

    }
}
