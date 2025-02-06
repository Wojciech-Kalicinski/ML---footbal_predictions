using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace FootballPredictions.Controllers
{
    public class HomeController : Controller
    {
        private readonly FootballDataService _footballDataService;

        public HomeController(FootballDataService footballDataService)
        {
            _footballDataService = footballDataService;
        }

        // Metoda do obs³ugi strony g³ównej
        public IActionResult Index()
        {
            return View();
        }

        // Metoda do pobierania ostatnich meczów
        [HttpPost]
        public async Task<IActionResult> GetLastMatches([FromBody] TeamRequest request)
        {
            try
            {
                if (request.TeamId <= 0)
                {
                    return Json(new { result = "Error: Team ID must be provided." });
                }

                // Pobierz ostatnie mecze dla wybranej dru¿yny
                var matches = await _footballDataService.GetLastMatchesAsync(request.TeamId);

                return Json(new { result = "Success", matches });
            }
            catch (Exception ex)
            {
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Klasa do mapowania danych z ¿¹dania JSON
        public class TeamRequest
        {
            public int TeamId { get; set; }
        }
    }
}