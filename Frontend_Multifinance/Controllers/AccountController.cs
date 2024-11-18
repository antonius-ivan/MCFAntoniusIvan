using Microsoft.AspNetCore.Mvc;
using Frontend_Multifinance.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Frontend_Multifinance.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public AccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login()
        {
            // Check if the user session exists, if not show the login page without the layout
            if (HttpContext.Session.GetString("UserData") == null)
            {
                // Indicate that layout should not be used for login page
                ViewData["UseLayout"] = false;
                return View();
            }
            // If session exists, redirect to the next page (e.g., Upsert)
            return RedirectToAction("Upsert", "BpkbTransaction");
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Prepare JSON content for the request
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send POST request to backend API for login
                var response = await _httpClient.PostAsync("https://localhost:9999/api/MsUser/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Authentication successful, read the response data (e.g., user details)
                    var userData = await response.Content.ReadAsStringAsync();

                    // Store user data in the session
                    HttpContext.Session.SetString("UserData", userData);

                    // Redirect to the Upsert page after successful login
                    return RedirectToAction("Index", "BpkbTransaction");
                }
                else
                {
                    // Authentication failed, show an error
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }
    }
}
