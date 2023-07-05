using CitiesManager.Core.Helpers;
using System.Text.Json;

namespace CitiesManager.WebAPI.StartupExtensions
{
    /// <summary>
    /// 25.06.2023. - Poziva ga Controller, npr CityController, a on poziva klasu PaginationHeader
    /// </summary>
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage,
            int itemsPerPage, int totalItems, int totalPages, bool hasNext, bool hasPrevious)
        {
            var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages, hasPrevious, hasNext);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, options));
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
        }
    }

}
