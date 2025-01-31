using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace FootballPredictionApp.Controllers
{
    public class PredictionsController : Controller
    {
        public async Task<IActionResult> Index()
        {
            // Ścieżka do skryptu Python
            string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\app.py";

            // Uruchomienie skryptu Python asynchronicznie
            string result = await RunPythonScriptAsync(pythonScriptPath);

            // Odczytaj wyniki z pliku CSV
            string predictionsFilePath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predictions.csv";
            if (System.IO.File.Exists(predictionsFilePath))
            {
                var predictions = System.IO.File.ReadAllLines(predictionsFilePath);
                ViewBag.Predictions = predictions;
            }
            else
            {
                ViewBag.Error = "Plik z wynikami nie został znaleziony.";
            }

            return View();
        }

        private async Task<string> RunPythonScriptAsync(string scriptPath)
        {
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "python",
                Arguments = scriptPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = processStartInfo })
            {
                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (!string.IsNullOrEmpty(error))
                {
                    ViewBag.Error = error;
                }

                return output;
            }
        }
    }
}