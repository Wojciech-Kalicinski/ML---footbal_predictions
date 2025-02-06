using Football_Predictions.Models;
using Football_Predictions.Services;

var builder = WebApplication.CreateBuilder(args);

// Dodaj konfiguracjê API
builder.Services.Configure<FootballDataApiSettings>(builder.Configuration.GetSection("FootballDataApi"));

// Dodaj HttpClient do obs³ugi ¿¹dañ HTTP
builder.Services.AddHttpClient<FootballDataService>();

// Dodaj kontrolery i widoki (jeœli u¿ywasz MVC)
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja pipeline'u ¿¹dañ HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Domyœlna wartoœæ HSTS to 30 dni. Mo¿esz to zmieniæ dla œrodowiska produkcyjnego.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Mapowanie domyœlnej trasy dla kontrolerów
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();