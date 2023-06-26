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
using OfficeOpenXml.ExternalReferences;
using System;
using System.Drawing.Printing;
using CitiesManager.Core.Helpers;

namespace CitiesManager.Core.Services
{
    /// <summary>
    /// Poziva ga Controller
    /// </summary>
    public class CitiesGetterService : ICitiesGetterService
    {
        //private field
        private readonly ICitiesRepository _citiesRepository;
        private readonly ILogger<CitiesGetterService> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        //constructor
        public CitiesGetterService(ICitiesRepository citiesRepository, ILogger<CitiesGetterService> logger, IDiagnosticContext diagnosticContext)
        {
            _citiesRepository = citiesRepository;
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public virtual async Task<List<CityResponse>> GetAllCities()
        {
            _logger.LogInformation("GetAllCities of CitiesService");

            var cities = await _citiesRepository.GetAllCities();

            return cities
              .Select(temp => temp.ToCityResponse()).ToList();
        }


        /// <summary>
        /// 23.06.2023. - zamenuo: pageNumber, pageSize sa CityParameters cityParameters
        /// 23.06.2023. - PagedList
        /// 25.06.2023. - Pagination + Filters + Sorting
        /// </summary>
        /// <param name="cityParameters"></param>
        /// <returns></returns>
        public virtual async Task<PagedList<CityResponse>> GetAllCitiesPg(CityParameters cityParameters)
        {
            _logger.LogInformation("GetAllCities of CitiesService Pagination");

            // Preuzmi gradove iz repozitorija 
            var cities = await _citiesRepository.GetAllCitiesPg(cityParameters);

            return cities;

        }


        public virtual async Task<CityResponse?> GetCityByCityID(Guid? cityID)
        {
            if (cityID == null)
                return null;

            City? city = await _citiesRepository.GetCityByCityID(cityID.Value);

            if (city == null)
                return null;

            return city.ToCityResponse();
        }


        public virtual async Task<List<CityResponse>> GetFilteredCities(string searchBy, string? searchString)
        {
            _logger.LogInformation("GetFilteredCities of CitiesService");

            List<City> cities;

            using (Operation.Time("Time for Filtered Cities from Database"))
            {
                cities = searchBy switch
                {
                    nameof(CityResponse.CityName) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.CityName!.Contains(searchString!)),

                    nameof(CityResponse.DateOfFoundation) =>
                    await _citiesRepository.GetFilteredCities(temp =>
                    temp.DateOfFoundation!.Value.ToString("dd MMM yyyy").Contains(searchString!)),

                    nameof(CityResponse.CityHistory) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.CityHistory!.Contains(searchString!)),

                    nameof(CityResponse.Population) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.Population.ToString().Contains(searchString!)),

                    nameof(CityResponse.ZipCode) =>
                    await _citiesRepository.GetFilteredCities(temp =>
                    temp.ZipCode!.Contains(searchString!)),

                    nameof(CityResponse.Description) =>
                     await _citiesRepository.GetFilteredCities(temp =>
                     temp.Description!.Contains(searchString!)),

                    _ => await _citiesRepository.GetAllCities()
                };
            } //end of "using block" of serilog timings

            _diagnosticContext.Set("Cities", cities);

            return cities.Select(temp => temp.ToCityResponse()).ToList();
        }


        public virtual async Task<MemoryStream> GetCitiesCSV()
        {
            MemoryStream memoryStream = new MemoryStream();
            StreamWriter streamWriter = new StreamWriter(memoryStream);

            CsvConfiguration csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture);
            CsvWriter csvWriter = new CsvWriter(streamWriter, csvConfiguration);

            //CityName
            csvWriter.WriteField(nameof(CityResponse.CityName));
            csvWriter.WriteField(nameof(CityResponse.DateOfFoundation));
            csvWriter.WriteField(nameof(CityResponse.CityHistory));
            csvWriter.WriteField(nameof(CityResponse.Population));
            csvWriter.WriteField(nameof(CityResponse.ZipCode));
            csvWriter.WriteField(nameof(CityResponse.Description));
            csvWriter.NextRecord();

            List<CityResponse> cities = await GetAllCities();

            foreach (CityResponse city in cities)
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
                List<CityResponse> cities = await GetAllCities();

                foreach (CityResponse city in cities)
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
    }
}
