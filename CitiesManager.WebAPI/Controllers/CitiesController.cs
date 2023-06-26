using Microsoft.AspNetCore.Mvc;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.DTO;
using System.Drawing.Printing;
using CitiesManager.Core.Domain.Entities;
using Newtonsoft.Json;
using CitiesManager.WebAPI.StartupExtensions;

namespace CitiesManager.WebAPI.Controllers
{
    /// <summary>
    /// Conntroler poziva Servis, Servis poziva Repository... između se svašta nešto poziva !!!
    /// Akcenat na GetCitiesPg, ovo treba očistiti kroz celu Clean Arch. tj sve projekte
    /// </summary>
    public class CitiesController : CustomControllerBase
    {

        //private fields
        private readonly ICitiesGetterService _citiesGetterService;
        private readonly ICitiesAdderService _citiesAdderService;
        private readonly ICitiesDeleterService _citiesDeleterService;
        private readonly ICitiesUpdaterService _citiesUpdaterService;

        private readonly ILogger<CitiesController> _logger;


        //constructor
        public CitiesController(ICitiesGetterService citiesGetterService, ICitiesAdderService citiesAdderService, ICitiesDeleterService citiesDeleterService, ICitiesUpdaterService citiesUpdaterService, ILogger<CitiesController> logger)
        {
            _citiesGetterService = citiesGetterService;
            _citiesAdderService = citiesAdderService;
            _citiesUpdaterService = citiesUpdaterService;
            _citiesDeleterService = citiesDeleterService;

            _logger = logger;
        }

        // GET: api/Cities
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityResponse>>> GetCities()
        {

            _logger.LogInformation("Index action method of CitiesController");

            List<CityResponse> cities = await _citiesGetterService.GetAllCities();

            return cities;
        }


        /// <summary>
        /// 24.06.2023.
        /// Pagination + Filters + Sorting
        /// </summary>
        /// <param name="cityParameters"></param>
        /// <returns></returns>
        [HttpGet("GetCitiesPg")]
        public async Task<ActionResult<IEnumerable<CityResponse>>> GetCitiesPg([FromQuery] CityParameters cityParameters)
        {

            _logger.LogInformation("HttpGet  method of CitiesController -  GetCitiesPg(int pageNumber, int pageSize)");

            var cities = await _citiesGetterService.GetAllCitiesPg(cityParameters);

            if (cities == null)
            {
                return NotFound();
            }

            // 25.04.2023. Korak_4 - klasa u WebAPI-StartupExtension folder 
            Response.AddPaginationHeader(cities.CurrentPage, cities.PageSize,
                cities.TotalCount, cities.TotalPages, cities.HasNext, cities.HasPrevious);


            _logger.LogInformation($"Returned {cities.TotalCount} cities from database");

            return Ok(cities);

        }

        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityResponse>> GetCity(Guid id)
        {

            var city = await _citiesGetterService.GetCityByCityID(id);

            if (city == null)
            {
                return NotFound();
            }

            return city;
        }

        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCity(Guid id, CityUpdateRequest cityRequest)
        {

            if (id != cityRequest.CityID)
            {
                return BadRequest();
            }
            CityResponse? cityResponse = await _citiesGetterService.GetCityByCityID(id);

            if (cityResponse == null)
            {
                return NotFound();
            }

            CityResponse updatedCity = await _citiesUpdaterService.UpdateCity(cityRequest);
            return NoContent();

        }

        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<CityResponse>> PostCity(CityAddRequest cityRequest)
        {

            if (cityRequest == null)
            {
                return Problem("Entity set 'CityAddRequest'  is null.");
            }

            //call the service method
            CityResponse cityResponse = await _citiesAdderService.AddCity(cityRequest);

            return CreatedAtAction("GetCity", new { id = cityResponse.CityID }, cityResponse);

        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {

            CityResponse? cityResponse = await _citiesGetterService.GetCityByCityID(id);
            if (cityResponse == null)
                return NotFound();

            await _citiesDeleterService.DeleteCity(id);
            return NoContent();
        }



        [HttpGet("CitiesCSV")]
        public async Task<IActionResult> CitiesCSV()
        {
            MemoryStream memoryStream = await _citiesGetterService.GetCitiesCSV();
            return File(memoryStream, "application/octet-stream", "cities.csv");
        }


        [HttpGet("CitiesExcel")]
        public async Task<IActionResult> CitiesExcel()
        {
            MemoryStream memoryStream = await _citiesGetterService.GetCitiesExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "cities.xlsx");
        }


        /// <summary>
        /// Ne koristim trenutno, ne treba mi bar za sada 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //private bool CityExists(Guid id)
        //{
        //    return (_context.Cities?.Any(e => e.CityID == id)).GetValueOrDefault();
        //}


        //
        // Ne koristim ga, nije dobro... moram naći drugo rešenje ???
        // možda da Angular kreira PDF, a možda može i sam Asp.net Core da ga kreira ???
        //
        //[HttpGet("CitiesPDF")]
        //public async Task<IActionResult> CitiesPDF()
        //{
        //    //Get list of cities
        //    List<CityResponse> cities = await _citiesGetterService.GetAllCities();

        //    //Return view as pdf
        //    return new ViewAsPdf("CitiesPDF", cities, ViewData)
        //    {
        //        PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Right = 20, Bottom = 20, Left = 20 },
        //        PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape
        //    };
        //}


    }
}
