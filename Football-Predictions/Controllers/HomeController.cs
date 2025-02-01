using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FootballPredictions.Controllers
{
    public class HomeController : Controller
    {
        // Metoda do obs³ugi strony g³ównej
        public IActionResult Index()
        {
            return View();
        }

        // Metoda do obs³ugi predykcji
        [HttpPost]
        public IActionResult Predict([FromBody] PredictionRequest request)
        {
            try
            {
                // SprawdŸ, czy argumenty nie s¹ puste
                if (string.IsNullOrEmpty(request.HomeTeam) || string.IsNullOrEmpty(request.AwayTeam))
                {
                    return Json(new { result = "Error: Home team and away team must be provided." });
                }

                // Œcie¿ka do skryptu Pythona
                string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predict.py";

                // Uruchom proces Pythona z odpowiednimi argumentami
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python", // Nazwa interpretera Pythona
                        Arguments = $"\"{pythonScriptPath}\" \"{request.HomeTeam}\" \"{request.AwayTeam}\"", // Przekazanie argumentów
                        RedirectStandardOutput = true, // Przekieruj wyjœcie
                        RedirectStandardError = true, // Przekieruj b³êdy
                        UseShellExecute = false, // Nie u¿ywaj pow³oki systemowej
                        CreateNoWindow = true, // Nie twórz okna konsoli
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd(); // Odczytaj wynik z Pythona
                string error = process.StandardError.ReadToEnd(); // Odczytaj b³êdy z Pythona
                process.WaitForExit();

                // Jeœli wyst¹pi³ b³¹d, zwróæ go
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { result = $"Python Error: {error}" });
                }

                // Usuñ niepotrzebne bia³e znaki z outputu i sprawdŸ wynik
                string prediction = output.Trim();

                // SprawdŸ, czy wynik to poprawna liczba (1, 0, -1)
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
                // Obs³u¿ b³êdy
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Klasa do mapowania danych z ¿¹dania JSON
        public class PredictionRequest
        {
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
        }
    }
}
