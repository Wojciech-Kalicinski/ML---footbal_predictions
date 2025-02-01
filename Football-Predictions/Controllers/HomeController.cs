[HttpPost]
public IActionResult Predict(string homeTeam, string awayTeam)
{
    try
    {
        // �cie�ka do skryptu Pythona
        string pythonScriptPath = @"C:\Users\48512\Documents\GitHub\ML---footbal_predictions\predict.py";

        // Uruchom proces Pythona z argumentami
        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "python", // Nazwa interpretera Pythona
                Arguments = $"\"{pythonScriptPath}\" \"{homeTeam}\" \"{awayTeam}\"", // Przekazanie �cie�ki i argument�w
                RedirectStandardOutput = true, // Przekieruj wyj�cie
                UseShellExecute = false, // Nie u�ywaj pow�oki systemowej
                CreateNoWindow = true, // Nie tworzy okna konsoli
            }
        };

        process.Start();
        string result = process.StandardOutput.ReadToEnd(); // Odczytaj wynik z Pythona
        process.WaitForExit();

        // Zwr�� wynik jako JSON
        return Json(new { result = result.Trim() }); // Trim() usuwa zb�dne bia�e znaki
    }
    catch (Exception ex)
    {
        // Obs�u� b��dy
        return Json(new { result = $"Error: {ex.Message}" });
    }
}