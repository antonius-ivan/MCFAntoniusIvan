using Frontend_Multifinance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend_Multifinance.Controllers
{
    public class BpkbTransactionController : Controller
    {
        private readonly HttpClient _httpClient;

        public BpkbTransactionController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        // Index Action to get the list of BPKB transactions
        public async Task<IActionResult> Index()
        {
            // Fetch the list of BPKB transactions from the API
            var transactions = await GetBpkbTransactionsAsync();

            // Pass the transactions to the view as the model
            return View(transactions);
        }

        private async Task<List<BpkbTransactionViewModel>> GetBpkbTransactionsAsync()
        {
            var transactions = new List<BpkbTransactionViewModel>();

            try
            {
                string apiUrl = "https://localhost:9999/api/BpkbTransaction"; // The API endpoint to fetch the list of transactions

                // Send GET request to fetch the list
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Deserialize JSON response into a list of BpkbTransactionViewModel objects
                var responseBody = await response.Content.ReadAsStringAsync();
                transactions = JsonSerializer.Deserialize<List<BpkbTransactionViewModel>>(responseBody);
            }
            catch (Exception ex)
            {
                // Log the error (you can replace this with proper logging)
                Console.WriteLine($"Error fetching transactions: {ex.Message}");
            }

            return transactions;
        }

        public async Task<IActionResult> Upsert()
        {
            // Fetch storage locations from the API
            var storageLocations = await GetStorageLocationsAsync();

            // Pass the locations to the View using ViewBag
            ViewBag.StorageLocations = storageLocations;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(BpkbTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Prepare data for API request
                    var jsonContent = JsonSerializer.Serialize(model);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    // API endpoint for saving/updating
                    string apiUrl = "https://localhost:9999/api/BpkbTransaction";

                    // Send POST request to save the data
                    var response = await _httpClient.PostAsync(apiUrl, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // Redirect to success page or list view after saving
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to save the data. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception (replace with proper logging)
                    Console.WriteLine($"Error while saving data: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            // Return the view with validation errors
            return View(model);
        }

        private async Task<List<SelectListItem>> GetStorageLocationsAsync()
        {
            var storageLocations = new List<SelectListItem>();
            try
            {
                // API endpoint
                string apiUrl = "https://localhost:9999/api/StorageLocation";

                // Send GET request to fetch data
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                // Deserialize JSON response into a list of objects
                var responseBody = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<StorageLocationViewModel>>(responseBody);

                // Map the response to a list of SelectListItem
                if (locations != null)
                {
                    foreach (var location in locations)
                    {
                        storageLocations.Add(new SelectListItem
                        {
                            Value = location.location_id,
                            Text = location.location_name
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error (you can replace this with proper logging)
                Console.WriteLine($"Error fetching storage locations: {ex.Message}");
            }

            return storageLocations;
        }
    }
}
