using Microsoft.AspNetCore.Mvc;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.DTO;
using CitiesManager.WebAPI.StartupExtensions;

namespace CitiesManager.WebAPI.Controllers
{
    /// <summary>
    /// Conntroller poziva Servis, Servis poziva Repository... između se svašta nešto poziva !!!
    /// 15.07.2023. - poziv i Validacije
    /// Akcenat na GetCitiesPg
    /// </summary>
    public class CitiesController : CustomControllerBase
    {

        //private fields
        private readonly ICitiesServiceCRUD _citiesServiceCRUD;
        private readonly ILogger<CitiesController> _logger;

        // 14.07.2023. Put TEST Ver 2
        //private readonly CityBOValidaros _cityBOValidators;

        /// <summary>
        /// 15.07.2023. - korekcija, Pogledaj komentare na POST i PUT
        /// </summary>
        /// <param name="citiesServiceCRUD"></param>
        /// <param name="logger"></param>
        public CitiesController(ICitiesServiceCRUD citiesServiceCRUD, ILogger<CitiesController> logger)
        {
            _citiesServiceCRUD = citiesServiceCRUD;
            _logger = logger;
            //_cityBOValidators = cityBOValidators;             // 14.07.2023.  Put TEST Ver 2
        }


        // GET: api/Cities
        /// <summary>
        /// 29.06.2023. - CityResponseRecord
        /// Umesto ovog poziva, koristi GetCitiesPg
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllCitiesRc")]
        public async Task<ActionResult<IEnumerable<CityResponseRecord>>> GetCitiesRc()
        {

            _logger.LogInformation("Index action method of CitiesController");

            List<CityResponseRecord> cities = await _citiesServiceCRUD.GetAllCitiesRc() ;

            return cities;
        }

        /// <summary>
        /// 24.06.2023. - Pagination + Filters + Sorting
        /// 03.07.2023. CityResponseRecord
        /// </summary>
        /// <param name="cityParameters"></param>
        /// <returns></returns>
        [HttpGet("GetCitiesPg")]
        public async Task<ActionResult<IEnumerable<CityResponseRecord>>> GetCitiesPg([FromQuery] CityParameters cityParameters)
        {

            _logger.LogInformation("HttpGet  method of CitiesController -  GetCitiesPg(int pageNumber, int pageSize)");

            var cities = await _citiesServiceCRUD.GetAllCitiesPg(cityParameters);

            if (cities is null)
            {
                return NotFound();
            }

            // 25.04.2023. Korak_4 - klasa u WebAPI-StartupExtension folder 
            Response.AddPaginationHeader(cities.CurrentPage, cities.PageSize,
                cities.TotalCount, cities.TotalPages, cities.HasNext, cities.HasPrevious);


            _logger.LogInformation($"Returned {cities.TotalCount} cities from database");

            return Ok(cities);

        }

        /// <summary>
        /// 03.07.2023. - CityResponseRecord
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Cities/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CityResponseRecord>> GetCity(Guid id)
        {

            var city = await _citiesServiceCRUD.GetCityByCityID(id);

            if (city is null)
            {
                return NotFound();
            }

            return city;
        }


        // POST: api/Cities
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 03.07.2023. - CityResponseRecord
        /// 15.07.2023. - korigovan poziv Validacije
        /// 17.07.2023. - sklonio validaciju
        /// </summary>
        /// <param name="cityRequest"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        [HttpPost]
        //public async Task<ActionResult<CityResponseRecord>> PostCity(CityAddRequest cityRequest, IValidator<CityAddRequest> validator)
        public async Task<ActionResult<CityResponseRecord>> PostCity(CityAddRequest cityRequest)
        {

            if (cityRequest is null)
            {
                return Problem("Entity set 'CityAddRequest'  is null.");
            }

            // 17.07.2023. - komentovao validaciju
            //// 15.07.2023. - korekcija validacije
            //ValidationResult result = await validator.ValidateAsync(cityRequest);
            //if (!result.IsValid)
            //{
            //    return BadRequest(result.ToDictionary());

            //    //var validationProblemDetails = new ValidationProblemDetails(result.ToDictionary())
            //    //{
            //    //    Title = "Validation Error",
            //    //    Status = 400,
            //    //    Detail = "One or more validation errors occurred."
            //    //};

            //    //return BadRequest(validationProblemDetails);
            //    //return ValidationProblem((ValidationProblemDetails)result.ToDictionary());
            //    //return Results.ValidationProblem(result.ToDictionary()); // grešku javlja
            //}

            //call the service method
            CityResponseRecord cityResponse = await _citiesServiceCRUD.AddCity(cityRequest);

            return CreatedAtAction("GetCity", new { id = cityResponse.CityID }, cityResponse);

        }


        // PUT: api/Cities/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        /// <summary>
        /// 15.07.2023. - korigovan poziv Validacije
        /// 17.07.2023. - sklonio validaciju
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cityRequest"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        //public async Task<ActionResult<CityResponseRecord>> PutCity(Guid id, CityUpdateRequest cityRequest,  IValidator<CityUpdateRequest> validator)
        public async Task<ActionResult<CityResponseRecord>> PutCity(Guid id, CityUpdateRequest cityRequest)
        {

            // 14.07.2023. test Ver_2 jedan Bo
            //bool vresult = await _cityBOValidators.IsCityUpdateValid(cityRequest);
            //if (!vresult )
            //{
            //   return BadRequest(_cityBOValidators.Errors);
            //}

            // UPDATE - 04.07.2023. dodao id
            var updatedCity = await _citiesServiceCRUD.UpdateCity(id, cityRequest);

            if (updatedCity is null)
            {
                return NotFound();
            }

            //return Ok(updatedCity);     // return NoContent();
            return CreatedAtAction("GetCity", new { id = updatedCity.CityID }, updatedCity);

        }

        // DELETE: api/Cities/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCity(Guid id)
        {

            // 11.07.2023. - MJ
            if (await _citiesServiceCRUD.DeleteCity(id))
            {
                return NoContent();
            }
            return NotFound();

        }



        [HttpGet("CitiesCSV")]
        public async Task<IActionResult> CitiesCSV()
        {
            MemoryStream memoryStream = await _citiesServiceCRUD.GetCitiesCSV();
            return File(memoryStream, "application/octet-stream", "cities.csv");
        }


        [HttpGet("CitiesExcel")]
        public async Task<IActionResult> CitiesExcel()
        {
            MemoryStream memoryStream = await _citiesServiceCRUD.GetCitiesExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "cities.xlsx");
        }

        // 15.07.2023.
        [HttpGet("CitiesExcelGeneric")]
        public async Task<IActionResult> CitiesExcelGeneric()
        {
            await _citiesServiceCRUD.CitiesExcelGeneric();
            return Ok();
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
