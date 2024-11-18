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

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Prepare JSON content
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send POST request to backend API
                var response = await _httpClient.PostAsync("https://localhost:9999/api/MsUser/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Authentication successful, read the response data (e.g., user details)
                    var userData = await response.Content.ReadAsStringAsync();

                    // You can optionally deserialize the response into an object if needed
                    // var user = JsonSerializer.Deserialize<User>(userData);

                    // Store user data in the session
                    HttpContext.Session.SetString("UserData", userData);

                    // Redirect to the Upsert page
                    return RedirectToAction("Upsert", "BpkbTransaction");
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
