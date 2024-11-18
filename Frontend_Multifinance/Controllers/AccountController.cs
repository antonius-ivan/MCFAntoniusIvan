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
            if (HttpContext.Session.GetString("UserData") == null)
            {
                ViewData["UseLayout"] = false;
                return View();
            }
            return RedirectToAction("Upsert", "BpkbTransaction");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var jsonContent = new StringContent(
                    JsonSerializer.Serialize(model),
                    Encoding.UTF8,
                    "application/json"
                );

                var response = await _httpClient.PostAsync("https://localhost:9999/api/MsUser/login", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var userData = await response.Content.ReadAsStringAsync();

                    HttpContext.Session.SetString("UserData", userData);

                    return RedirectToAction("Index", "BpkbTransaction");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }
    }
}
