var builder = WebApplication.CreateBuilder(args);

// Dodaj us³ugi do kontenera DI.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja potoku ¿¹dañ HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Mapowanie domyœlnej trasy
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Predictions}/{action=Index}/{id?}");

app.Run();