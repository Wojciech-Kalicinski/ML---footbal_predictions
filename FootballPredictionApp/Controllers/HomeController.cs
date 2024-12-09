using Microsoft.AspNetCore.Mvc;
using FootballPredictionApp.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FootballPredictionApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Predict()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Predict([FromBody] PredictModel model)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = "http://127.0.0.1:5000/predict"; // Adres backendu Python
                var json = JsonSerializer.Serialize(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(apiUrl, content);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    return Content(result, "application/json");
                }
                else
                {
                    return StatusCode((int)response.StatusCode, "Error calling Python API.");
                }
            }

            return BadRequest("Invalid model.");
        }
    }
}
