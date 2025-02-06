using Football_Predictions.Models;
using Football_Predictions.Services;

var builder = WebApplication.CreateBuilder(args);

// Dodaj konfiguracj� API
builder.Services.Configure<FootballDataApiSettings>(builder.Configuration.GetSection("FootballDataApi"));

// Dodaj HttpClient do obs�ugi ��da� HTTP
builder.Services.AddHttpClient<FootballDataService>();

// Dodaj kontrolery i widoki (je�li u�ywasz MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja pipeline'u ��da� HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Domy�lna warto�� HSTS to 30 dni. Mo�esz to zmieni� dla �rodowiska produkcyjnego.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapowanie domy�lnej trasy dla kontroler�w
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();