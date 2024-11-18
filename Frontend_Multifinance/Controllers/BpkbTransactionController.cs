using Frontend_Multifinance.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Net;
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
            // Fetch list of BPKB transactions
            var transactions = await GetBpkbTransactionsAsync();

            // Fetch storage locations
            var storageLocations = await GetStorageLocationsAsync();

            // Pass the list of storage locations to the view
            ViewBag.StorageLocations = storageLocations;

            return View(transactions);
        }
        // Action to show the details of a specific BPKB transaction
        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var transaction = await GetBpkbTransactionByIdAsync(id);

            if (transaction == null)
            {
                return NotFound();
            }

            // Assuming GetStorageLocationsAsync fetches the list of storage locations
            var storageLocations = await GetStorageLocationsAsync();

            ViewBag.StorageLocations = storageLocations;

            return View(transaction);
        }

        // Fetching a specific transaction by agreement number
        private async Task<BpkbTransactionViewModel> GetBpkbTransactionByIdAsync(string id)
        {
            try
            {
                string apiUrl = $"https://localhost:9999/api/BpkbTransaction/{id}"; // Endpoint to fetch by ID

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var transaction = JsonSerializer.Deserialize<BpkbTransactionViewModel>(responseBody);

                return transaction;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transaction details: {ex.Message}");
                return null;
            }
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

        // Action to display the Upsert form
        // Consolidating both methods into one
        public async Task<IActionResult> Upsert(string id)
        {
            BpkbTransactionViewModel model = new BpkbTransactionViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                // Fetch the existing transaction data from the API if 'id' is provided
                model = await _httpClient.GetFromJsonAsync<BpkbTransactionViewModel>($"https://localhost:9999/api/BpkbTransaction/{id}");
            }
            else
            {
                // If no id is provided, you can initialize the model for a new transaction (if needed).
                var newAgreementNumber = await GenerateNewAgreementNumberAsync();
                model.agreement_number = newAgreementNumber;
            }

            // Fetch storage locations for the dropdown list in the form
            var storageLocations = await GetStorageLocationsAsync();
            ViewBag.StorageLocations = storageLocations;

            // Pass the model (either fetched or a new one) to the view
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(BpkbTransactionViewModel model)
        {
            // Ensure the model is valid before proceeding
            if (ModelState.IsValid)
            {
                try
                {
                    // Check if the agreement_number exists in the database by making a GET request
                    var response = await _httpClient.GetAsync($"https://localhost:9999/api/BpkbTransaction/{model.agreement_number}");

                    if (response.IsSuccessStatusCode)
                    {
                        // Transaction exists, update it using a PUT request
                        var updateResponse = await _httpClient.PutAsJsonAsync($"https://localhost:9999/api/BpkbTransaction/{model.agreement_number}", model);

                        if (updateResponse.IsSuccessStatusCode)
                        {
                            // Redirect to the Index action after successful update
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Handle failed update, show an error message
                            ModelState.AddModelError("", "Failed to update the transaction. Please try again.");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        // If the transaction is not found, create a new one using POST request
                        var createResponse = await _httpClient.PostAsJsonAsync("https://localhost:9999/api/BpkbTransaction", model);

                        if (createResponse.IsSuccessStatusCode)
                        {
                            // Redirect to the Index action after successful creation
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            // Handle failed creation, show an error message
                            ModelState.AddModelError("", "Failed to create the transaction. Please try again.");
                        }
                    }
                    else
                    {
                        // Handle unexpected responses (e.g., server error, etc.)
                        ModelState.AddModelError("", "An error occurred while processing your request.");
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception and add an error to the ModelState
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            // If validation fails or there's an error, return the view with the model
            return View(model);
        }

        // Example method to fetch the transaction details by agreement number
        private async Task<BpkbTransactionViewModel> GetTransactionByAgreementNumberAsync(string agreementNumber)
        {
            // Replace this with your actual method to get the transaction details from a database or API
            return new BpkbTransactionViewModel
            {
                agreement_number = agreementNumber,
                bpkb_no = "BPKB12345",
                branch_id = "Branch1",
                bpkb_date = DateTime.Now,
                faktur_no = "Faktur12345",
                faktur_date = DateTime.Now,
                location_id = "1",
                police_no = "XYZ123"
            };
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

        // Method to generate a new agreement number
        private async Task<string> GenerateNewAgreementNumberAsync()
        {
            // This logic assumes the latest agreement number is retrieved from the database and incremented
            var lastTransaction = await GetLastTransactionAsync();

            // Assuming agreement_number is numeric and we increment it
            int newAgreementNumber = 0;
            if (lastTransaction != null && int.TryParse(lastTransaction.agreement_number, out newAgreementNumber))
            {
                newAgreementNumber++;
            }
            else
            {
                // Default to some starting number if no previous transactions exist
                newAgreementNumber = 1151500001;
            }

            return newAgreementNumber.ToString();
        }

        // Fetch the latest transaction to generate a new agreement number
        private async Task<BpkbTransactionViewModel> GetLastTransactionAsync()
        {
            var transactions = await GetBpkbTransactionsAsync();
            return transactions?.OrderByDescending(t => t.agreement_number).FirstOrDefault();
        }
    }
}
