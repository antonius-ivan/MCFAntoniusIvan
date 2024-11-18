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

        public async Task<IActionResult> Index()
        {
            var transactions = await GetBpkbTransactionsAsync();

            var storageLocations = await GetStorageLocationsAsync();

            ViewBag.StorageLocations = storageLocations;

            return View(transactions);
        }
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

            var storageLocations = await GetStorageLocationsAsync();

            ViewBag.StorageLocations = storageLocations;

            return View(transaction);
        }

        private async Task<BpkbTransactionViewModel> GetBpkbTransactionByIdAsync(string id)
        {
            try
            {
                string apiUrl = $"https://localhost:9999/api/BpkbTransaction/{id}"; 

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
                string apiUrl = "https://localhost:9999/api/BpkbTransaction"; 

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                transactions = JsonSerializer.Deserialize<List<BpkbTransactionViewModel>>(responseBody);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching transactions: {ex.Message}");
            }

            return transactions;
        }

        public async Task<IActionResult> Upsert(string id)
        {
            BpkbTransactionViewModel model = new BpkbTransactionViewModel();

            if (!string.IsNullOrEmpty(id))
            {
                model = await _httpClient.GetFromJsonAsync<BpkbTransactionViewModel>($"https://localhost:9999/api/BpkbTransaction/{id}");
            }
            else
            {
                var newAgreementNumber = await GenerateNewAgreementNumberAsync();
                model.agreement_number = newAgreementNumber;
            }

            var storageLocations = await GetStorageLocationsAsync();
            ViewBag.StorageLocations = storageLocations;

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(BpkbTransactionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"https://localhost:9999/api/BpkbTransaction/{model.agreement_number}");

                    if (response.IsSuccessStatusCode)
                    {
                        var updateResponse = await _httpClient.PutAsJsonAsync($"https://localhost:9999/api/BpkbTransaction/{model.agreement_number}", model);

                        if (updateResponse.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to update the transaction. Please try again.");
                        }
                    }
                    else if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        var createResponse = await _httpClient.PostAsJsonAsync("https://localhost:9999/api/BpkbTransaction", model);

                        if (createResponse.IsSuccessStatusCode)
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Failed to create the transaction. Please try again.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "An error occurred while processing your request.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                    ModelState.AddModelError("", "An unexpected error occurred.");
                }
            }

            return View(model);
        }

        private async Task<BpkbTransactionViewModel> GetTransactionByAgreementNumberAsync(string agreementNumber)
        {
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
                string apiUrl = "https://localhost:9999/api/StorageLocation";

                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var responseBody = await response.Content.ReadAsStringAsync();
                var locations = JsonSerializer.Deserialize<List<StorageLocationViewModel>>(responseBody);

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
                Console.WriteLine($"Error fetching storage locations: {ex.Message}");
            }

            return storageLocations;
        }

        private async Task<string> GenerateNewAgreementNumberAsync()
        {
            var lastTransaction = await GetLastTransactionAsync();

            int newAgreementNumber = 0;
            if (lastTransaction != null && int.TryParse(lastTransaction.agreement_number, out newAgreementNumber))
            {
                newAgreementNumber++;
            }
            else
            {
                newAgreementNumber = 1151500001;
            }

            return newAgreementNumber.ToString();
        }

        private async Task<BpkbTransactionViewModel> GetLastTransactionAsync()
        {
            var transactions = await GetBpkbTransactionsAsync();
            return transactions?.OrderByDescending(t => t.agreement_number).FirstOrDefault();
        }
    }
}
