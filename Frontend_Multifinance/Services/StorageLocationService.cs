using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Frontend_Multifinance.Services
{
    public class StorageLocationService
    {
        private readonly HttpClient _httpClient;

        public StorageLocationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<SelectListItem>> GetStorageLocationsAsync()
        {
            var storageLocations = new List<SelectListItem>();
            try
            {
                // API URL
                string apiUrl = "https://localhost:9999/api/StorageLocation";

                // Send GET request
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Deserialize JSON response
                var responseBody = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<StorageLocation>>(responseBody);

                // Map to SelectListItem
                if (locations != null)
                {
                    foreach (var location in locations)
                    {
                        storageLocations.Add(new SelectListItem
                        {
                            Value = location.LocationId,
                            Text = location.LocationName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle error (e.g., log the exception)
                Console.WriteLine($"Error fetching storage locations: {ex.Message}");
            }

            return storageLocations;
        }
    }

    // Model for the API response
    public class StorageLocation
    {
        public string LocationId { get; set; }
        public string LocationName { get; set; }
    }
}
