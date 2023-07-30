using Microsoft.AspNetCore.Mvc;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.DTO;
using CitiesManager.WebAPI.StartupExtensions;

namespace CitiesManager.WebAPI.Controllers
{
    /// <summary>
    /// The Controller calls the Service, the Service calls the Repository...
    /// </summary>
    public class CitiesController : CustomControllerBase
    {

        //private fields
        private readonly ICitiesServiceCRUD _citiesServiceCRUD;
        private readonly ILogger<CitiesController> _logger;

        // 14.07.2023. Put TEST Ver 2
        //private readonly CityBOValidaros _cityBOValidators;

        /// <summary>
        /// 15.07.2023. - corrections, see comments on POST and PUT !!!
        /// </summary>
        /// <param name="citiesServiceCRUD"></param>
        /// <param name="logger"></param>
        public CitiesController(ICitiesServiceCRUD citiesServiceCRUD, ILogger<CitiesController> logger)
        {
            _citiesServiceCRUD = citiesServiceCRUD;
            _logger = logger;
            //_cityBOValidators = cityBOValidators;     // 14.07.2023.  Put TEST Ver 2
        }


        // GET: api/Cities
        /// <summary>
        /// 29.06.2023. - CityResponseRecord
        /// Instead of this call, uses GetCitiesPg
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

            // 25.04.2023. Korak_4 - class in WebAPI-StartupExtension folder 
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
        /// 15.07.2023. - corrected call to Validation
        /// 17.07.2023. - removed the validation
        /// 20.07.2023. - Instead that, I'm trying to use the generic IMiddleware class
        /// Take a look at next classes:
        /// FluentValidationMiddleware.cs:
        /// - ValidatorMiddleware<TModel> : IMiddleware where TModel : class 
        /// CityUpdateValidator.cs
        /// CityAddValidator.cs
        /// Register the validators and middleware:
        /// - static class ConfigureServicesExtension
        /// - Program.cs ... app.UseWhen(...)
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

            //// 15.07.2023. - Corrected call to Validation
            //// 17.07.2023. - Comment the validation
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
        /// 15.07.2023. - corrected call to Validation
        /// 17.07.2023. - removed the validation
        /// 20.07.2023. - Instead that, I'm trying to use the generic IMiddleware class
        /// Take a look at next classes:
        /// FluentValidationMiddleware.cs:
        /// - ValidatorMiddleware<TModel> : IMiddleware where TModel : class 
        /// CityUpdateValidator.cs
        /// CityAddValidator.cs
        /// Register the validators and middleware:
        /// - static class ConfigureServicesExtension
        /// - Program.cs ... app.UseWhen(...)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cityRequest"></param>
        /// <param name="validator"></param>
        /// <returns></returns>
        //public async Task<ActionResult<CityResponseRecord>> PutCity(Guid id, CityUpdateRequest cityRequest,  IValidator<CityUpdateRequest> validator)
        [HttpPut("{id}")]
        public async Task<ActionResult<CityResponseRecord>> PutCity(Guid id, [FromBody] CityUpdateRequest cityRequest)
        {

            // 28.07.2023.
            if (cityRequest == null)
            {
                return BadRequest("JSON tokeni nisu prisutni u zahtevu.");
            }
           
            // UPDATE - 04.07.2023. Add id
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

            // 11.07.2023. 
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
        /// I'm not using right now
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //private bool CityExists(Guid id)
        //{
        //    return (_context.Cities?.Any(e => e.CityID == id)).GetValueOrDefault();
        //}


        //
        // I'm not using right now
        // maybe Angular to create the PDF, or maybe Asp.net Core itself to create it ???
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
