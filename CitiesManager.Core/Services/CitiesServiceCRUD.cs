using CsvHelper.Configuration;
using CsvHelper;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using Serilog;
using SerilogTimings;
using System.Globalization;
using CitiesManager.Core.Domain.RepositoryContracts;
using CitiesManager.Core.DTO;
using CitiesManager.Core.Domain.Entities;
using CitiesManager.Core.ServiceContracts;
using CitiesManager.Core.Helpers;
using CitiesManager.Core.Helpers.Validators;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace CitiesManager.Core.Services
{
    /// <summary>
    /// Poziva ga Controller
    /// 
    /// GET: 
    /// Task<List<CityResponseRecord>> GetAllCitiesRc()
    /// Task<PagedList<CityResponseRecord>> GetAllCitiesPg(CityParameters cityParameters)
    /// Task<CityResponseRecord?> GetCityByCityID(Guid? cityID)
    /// Task<List<CityResponseRecord>> GetFilteredCities(string searchBy, string? searchString)
    /// Task<MemoryStream> GetCitiesCSV()
    /// Task<MemoryStream> GetCitiesExcel()
    /// 
    /// </summary>
    public class CitiesServiceCRUD : ICitiesServiceCRUD
    {
        //private field
        private readonly ICitiesRepository _citiesRepository;
        private readonly ILogger<CitiesServiceCRUD> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public CitiesServiceCRUD(ICitiesRepository citiesRepository, ILogger<CitiesServiceCRUD> logger, IDiagnosticContext diagnosticContext)
        {
            _citiesRepository = citiesRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }


        /// <summary>
        /// 29.06.2023. - Milan Jovanović -Umesto CityResponse da probam sa CityresponseRecord
        /// </summary>
        /// <returns></returns>
        public virtual async Task<List<CityResponseRecord>> GetAllCitiesRc()
        {
            _logger.LogInformation("GetAllCities of CitiesService");

            var cities = await _citiesRepository.GetAllCities();

            // 03.07.2023. - Ostavio i ova jednostavna rešenja "peške" pisana, za nauk  !!!
            //var citiesRcs = cities
            //    .Select(temp => new CityResponseRecord(
            //        temp.CityID,
            //        temp.CityName,
            //        temp.DateOfFoundation,
            //        temp.CityHistory,
            //        temp.Population,
            //        temp.ZipCode,
            //        temp.Description
            //)).ToList();

            //return citiesRcs;

            //return cities
            // .Select(temp => temp.ToCityResponseRecord()).ToList();

            // 01.07.2023. - Milan Jovanović
            return cities.Select(CityExtensionsRecord.ToCityResponseRecord).ToList();

        }


        /// <summary>
        /// 23.06.2023. - zamenuo: pageNumber, pageSize sa CityParameters cityParameters
        /// 23.06.2023. - PagedList
        /// 25.06.2023. - Pagination + Filters + Sorting
        /// 03.07.2023. - CityResponseRecord
        /// </summary>
        /// <param name="cityParameters"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<CityResponseRecord>> GetAllCitiesPg(CityParameters cityParameters)
        {
            _logger.LogInformation("GetAllCities of CitiesService Pagination");

            // Preuzmi gradove iz repozitorija 
            var cities = await _citiesRepository.GetAllCitiesPg(cityParameters);

            return cities;

        }

        /// <summary>
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns>CityResponseRecord</returns>
        public virtual async Task<CityResponseRecord?> GetCityByCityID(Guid? cityID)
        {
            if (cityID == null)
                return null;

            City? city = await _citiesRepository.GetCityByCityID(cityID.Value);

            if (city == null)
                return null;

            return city.ToCityResponseRecord();
        }

        /// <summary>
        /// 03.07.2023. CityResponseRecord
        /// NE KORISTIM GA u ovakvoj situaciji, ali neka ostane kao primer, dobar je kao primer 
        /// Umesto njega koristim GetAllCitiesPg(CityParameters cityParameters)
        /// </summary>
        /// <param name="searchBy"></param>
        /// <param name="searchString"></param>
        /// <returns></returns>
        public virtual async Task<List<CityResponseRecord>> GetFilteredCities(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredCities of CitiesService");

            List<City> cities;

            using (Operation.Time("Time for Filtered Cities from Database"))
            {
                cities = searchBy switch
                {
                    nameof(CityResponseRecord.CityName) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.CityName!.Contains(searchString!)),

                    nameof(CityResponseRecord.DateOfFoundation) =>
                    await _citiesRepository.GetFilteredCities(temp =>
                    temp.DateOfFoundation!.Value.ToString("dd MMM yyyy").Contains(searchString!)),

                    nameof(CityResponseRecord.CityHistory) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.CityHistory!.Contains(searchString!)),

                    nameof(CityResponseRecord.Population) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.Population.ToString().Contains(searchString!)),

                    nameof(CityResponseRecord.ZipCode) =>
                    await _citiesRepository.GetFilteredCities(temp =>
                    temp.ZipCode!.Contains(searchString!)),

                    nameof(CityResponseRecord.Description) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.Description!.Contains(searchString!)),

                    _ => await _citiesRepository.GetAllCities()
                };
            } //end of "using block" of serilog timings

            _diagnosticContext.Set("Cities", cities);

            return cities.Select(CityExtensionsRecord.ToCityResponseRecord).ToList();
        }


        public virtual async Task<MemoryStream> GetCitiesCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            csvWriter.WriteField(nameof(CityResponseRecord.CityName));
            csvWriter.WriteField(nameof(CityResponseRecord.DateOfFoundation));
            csvWriter.WriteField(nameof(CityResponseRecord.CityHistory));
            csvWriter.WriteField(nameof(CityResponseRecord.Population));
            csvWriter.WriteField(nameof(CityResponseRecord.ZipCode));
            csvWriter.WriteField(nameof(CityResponseRecord.Description));
            csvWriter.NextRecord();

            List<CityResponseRecord> cities = await GetAllCitiesRc();

            foreach (CityResponseRecord city in cities)
            {
                csvWriter.WriteField(city.CityName);
                if (city.DateOfFoundation.HasValue)
                    csvWriter.WriteField(city.DateOfFoundation.Value.ToString("yyyy-MMM-dd"));
                else
                {
                    csvWriter.WriteField("");
                }
                csvWriter.WriteField(city.CityHistory);
                csvWriter.WriteField(city.Population);
                csvWriter.WriteField(city.ZipCode);
                csvWriter.WriteField(city.Description);
                csvWriter.NextRecord();
                csvWriter.Flush();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// https://zzzcode.ai/answer-question?p1=epplus&p2=how+to+set+columnwidth
        /// </summary>
        /// <returns></returns>
        public virtual async Task<MemoryStream> GetCitiesExcel()
        {
            MemoryStream memoryStream = new MemoryStream();
            using (ExcelPackage excelPackage = new ExcelPackage(memoryStream))
            {
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets.Add("CitiesSheet");

                // 22.06.2023.
                //workSheet.Column(3).Style.WrapText = true;

                workSheet.Cells["A1"].Value = "City Name";
                workSheet.Cells["B1"].Value = "Date Of Foundation";
                workSheet.Cells["C1"].Value = "City History";
                workSheet.Cells["D1"].Value = "City Population";
                workSheet.Cells["E1"].Value = "City ZipCode";
                workSheet.Cells["F1"].Value = "City Desription";

                using (ExcelRange headerCells = workSheet.Cells["A1:F1"])
                {
                    headerCells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    headerCells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    headerCells.Style.Font.Bold = true;

                    // 22.06.2023.
                    headerCells.AutoFilter = true;

                }

                int row = 2;
                List<CityResponseRecord> cities = await GetAllCitiesRc();

                foreach (CityResponseRecord city in cities)
                {
                    workSheet.Cells[row, 1].Value = city.CityName;


                    if (city.DateOfFoundation.HasValue)
                        workSheet.Cells[row, 2].Value = city.DateOfFoundation.Value.ToString("yyyy-MM-dd");
                    else
                    {
                        workSheet.Cells[row, 2].Value = "";
                    }

                    workSheet.Cells[row, 3].Value = city.CityHistory;
                    workSheet.Cells[row, 4].Value = city.Population;
                    workSheet.Cells[row, 5].Value = city.ZipCode;
                    workSheet.Cells[row, 6].Value = city.Description;

                    row++;
                }

                // 22.06.2023.
                //workSheet.Cells[$"A1:F{row}"].AutoFitColumns();
                workSheet.Cells.AutoFitColumns();
                workSheet.Column(3).Style.WrapText = true;
                workSheet.Column(3).Width = 70;

                await excelPackage.SaveAsync();
            }

            memoryStream.Position = 0;
            return memoryStream;
        }


        /// <summary>
        /// 15.07.2023.
        /// </summary>
        /// <returns></returns>
        public async Task CitiesExcelGeneric()
        {

            List<CityResponseRecord> cities = await GetAllCitiesRc();

            // Generisanje imena eksel fajla
            string mojNaslovXlsx = "Gradovi";
            
            string imefajlaTimestamp = mojNaslovXlsx+"_"+ DateTime.Now.ToString("yyyyMMddHHmmss")+".xlsx";
            
            string putanjaDoExcela = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads",imefajlaTimestamp);

            string sheetName = "Cities";

            ExportModelToExcelGeneric.ExportToExcel(cities, putanjaDoExcela, sheetName);

            
        }

        /// <summary>
        /// 03.07.2023. - Uklonio CityDeleteService
        /// </summary>
        /// <param name="cityID"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> DeleteCity(Guid? cityID)
        {
            if (cityID == null)
            {
                throw new ArgumentNullException(nameof(cityID));
            }

            // vratiće true/false
            return await (_citiesRepository.DeleteCityByCityID(cityID.Value));

        }

        /// <summary>
        /// 03.07.2023. - Uklonio CitiesAdderService
        /// </summary>
        /// <param name="cityAddRequest"></param>
        /// <returns>CityResponseRecord</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<CityResponseRecord> AddCity(CityAddRequest? cityAddRequest)
        {
            //check if CityAddRequest is not null
            if (cityAddRequest == null)
            {
                throw new ArgumentNullException(nameof(cityAddRequest));
            }

            //Model validation - 12.07.2023.
            //ValidationHelperH.ModelValidation(cityAddRequest);

            //convert cityAddRequest into City type
            City city = cityAddRequest.ToCity();

            //generate CityID
            city.CityID = Guid.NewGuid();

            //add city object to cities list
            await _citiesRepository.AddCity(city);

            //convert the City object into CityResponse type
            return city.ToCityResponseRecord();
        }

        /// <summary>
        /// 03.07.2023. - Uklonio CitiesUpdaterService
        /// 11.07.2023. - drugačiji pristup, manje cimanja BP
        /// Primedba od M.Jovanovića, slična kao i za Delete
        /// Ovo mi je najsporniji deo i nisam baš siguran da je urađeno baš kako i treba ???
        /// Svaka sugestija, pomoć je dobrodošla ...
        /// </summary>
        /// <param name="id"></param>
        /// <param name="cityUpdateRequest"></param>
        /// <returns>CityResponseRecord</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<CityResponseRecord?> UpdateCity(Guid id, CityUpdateRequest? cityUpdateRequest)
        {
            if (cityUpdateRequest == null)
                throw new ArgumentNullException(nameof(cityUpdateRequest));

            //validation - 12.07.2023.
            //ValidationHelperH.ModelValidation(cityUpdateRequest);

            // 11.07.2023.
            cityUpdateRequest.CityID = (cityUpdateRequest.CityID == Guid.Empty) ? id : cityUpdateRequest.CityID;

            var updateCity = await _citiesRepository.UpdateCity(cityUpdateRequest.ToCity()); //UPDATE
            CityResponseRecord? responseCity = (updateCity is not null) ? updateCity.ToCityResponseRecord() : null;

            return responseCity;

        }



    }
}
