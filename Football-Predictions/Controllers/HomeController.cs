using Football_Predictions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Football_Predictions.Controllers
{
    public class HomeController : Controller
    {
        private readonly FootballDataService _footballDataService;

        public HomeController(FootballDataService footballDataService)
        {
            _footballDataService = footballDataService;
        }

        // Main page handler
        public IActionResult Index()
        {
            return View();
        }

        // Method to fetch last matches of a team
        [HttpPost]
        public async Task<IActionResult> GetLastMatches([FromBody] TeamRequest request)
        {
            try
            {
                if (request.TeamId <= 0)
                {
                    return Json(new { result = "Error: Team ID must be provided." });
                }

                // Fetch last matches for the selected team
                var matches = await _footballDataService.GetLastMatchesAsync(request.TeamId);

                return Json(new { result = "Success", matches });
            }
            catch (Exception ex)
            {
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Method to get league standings
        [HttpPost]
        public async Task<IActionResult> GetLeagueStandings()
        {
            try
            {
                var standings = await _footballDataService.GetLeagueStandingsAsync();
                return Json(new { result = "Success", standings });
            }
            catch (Exception ex)
            {
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Prediction method that runs a Python script
        [HttpPost]
        public IActionResult Predict([FromBody] PredictionRequest request)
        {
            try
            {
                // Check if input arguments are valid
                if (string.IsNullOrEmpty(request.HomeTeam) || string.IsNullOrEmpty(request.AwayTeam))
                {
                    return Json(new { result = "Error: Home team and away team must be provided." });
                }

                // Path to the Python script (ensure this path is correct in your environment)
                string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predict.py";

                // Start a Python process with the required arguments
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python", // Python interpreter
                        Arguments = $"\"{pythonScriptPath}\" \"{request.HomeTeam}\" \"{request.AwayTeam}\"",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd(); // Read Python script output
                string error = process.StandardError.ReadToEnd(); // Read any error messages
                process.WaitForExit();

                // Handle Python script errors
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { result = $"Python Error: {error}" });
                }

                // Trim and validate the output
                string prediction = output.Trim();
                if (prediction == "1" || prediction == "0" || prediction == "-1")
                {
                    return Json(new { result = prediction });
                }
                else
                {
                    return Json(new { result = $"Error: Unexpected output from Python script: {output}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Data models for API requests
        public class TeamRequest
        {
            public int TeamId { get; set; }
        }

        public class PredictionRequest
        {
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
        }
    }
}
