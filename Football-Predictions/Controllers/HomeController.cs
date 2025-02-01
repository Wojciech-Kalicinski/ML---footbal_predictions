[HttpPost]
public IActionResult Predict(string homeTeam, string awayTeam)
{
    try
    {
        // Œcie¿ka do skryptu Pythona
        string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predict.py";

        // Uruchom proces Pythona z argumentami
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python", // Nazwa interpretera Pythona
                Arguments = $"\"{pythonScriptPath}\" \"{homeTeam}\" \"{awayTeam}\"", // Przekazanie œcie¿ki i argumentów
                RedirectStandardOutput = true, // Przekieruj wyjœcie
                UseShellExecute = false, // Nie u¿ywaj pow³oki systemowej
                CreateNoWindow = true, // Nie tworzy okna konsoli
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd(); // Odczytaj wynik z Pythona
        process.WaitForExit();

        // Zwróæ wynik jako JSON
        return Json(new { result = result.Trim() }); // Trim() usuwa zbêdne bia³e znaki
    }
    catch (Exception ex)
    {
        // Obs³u¿ b³êdy
        return Json(new { result = $"Error: {ex.Message}" });
    }
}