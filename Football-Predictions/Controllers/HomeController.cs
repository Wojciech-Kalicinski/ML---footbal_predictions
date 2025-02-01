using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FootballPredictions.Controllers
{
    public class HomeController : Controller
    {
        // Metoda do obs�ugi strony g��wnej
        public IActionResult Index()
        {
            return View();
        }

        // Metoda do obs�ugi predykcji
        [HttpPost]
        public IActionResult Predict([FromBody] PredictionRequest request)
        {
            try
            {
                // Sprawd�, czy argumenty nie s� puste
                if (string.IsNullOrEmpty(request.HomeTeam) || string.IsNullOrEmpty(request.AwayTeam))
                {
                    return Json(new { result = "Error: Home team and away team must be provided." });
                }

                // �cie�ka do skryptu Pythona
                string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predict.py";

                // Uruchom proces Pythona z odpowiednimi argumentami
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python", // Nazwa interpretera Pythona
                        Arguments = $"\"{pythonScriptPath}\" \"{request.HomeTeam}\" \"{request.AwayTeam}\"", // Przekazanie argument�w
                        RedirectStandardOutput = true, // Przekieruj wyj�cie
                        RedirectStandardError = true, // Przekieruj b��dy
                        UseShellExecute = false, // Nie u�ywaj pow�oki systemowej
                        CreateNoWindow = true, // Nie tw�rz okna konsoli
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd(); // Odczytaj wynik z Pythona
                string error = process.StandardError.ReadToEnd(); // Odczytaj b��dy z Pythona
                process.WaitForExit();

                // Je�li wyst�pi� b��d, zwr�� go
                if (!string.IsNullOrEmpty(error))
                {
                    return Json(new { result = $"Python Error: {error}" });
                }

                // Usu� niepotrzebne bia�e znaki z outputu i sprawd� wynik
                string prediction = output.Trim();

                // Sprawd�, czy wynik to poprawna liczba (1, 0, -1)
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
                // Obs�u� b��dy
                return Json(new { result = $"Error: {ex.Message}" });
            }
        }

        // Klasa do mapowania danych z ��dania JSON
        public class PredictionRequest
        {
            public string HomeTeam { get; set; }
            public string AwayTeam { get; set; }
        }
    }
}
