using CitiesManager.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitiesManager.Core.Helpers.Validators
{
    public interface ICityBOValidaros
    {
        /// <summary>
        /// 14.07.2023.
        /// Klasična BO validacija koja ispituje sve šta je potrebno, nije ograničena na attribute.... šta više, oni mogu o da se zanemare.
        /// Cilj: sve validacije za CityUpdateRequest u jednom BO
        /// </summary>
        /// <param name="cityUpdateRequest"></param>
        /// <returns></returns>
        Task<bool> IsCityUpdateValid(CityUpdateRequest? cityUpdateRequest);

    }
}
