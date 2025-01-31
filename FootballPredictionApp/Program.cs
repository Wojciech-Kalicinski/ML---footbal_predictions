var builder = WebApplication.CreateBuilder(args);

// Dodaj us�ugi do kontenera DI.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Konfiguracja potoku ��da� HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

// Mapowanie domy�lnej trasy
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Predictions}/{action=Index}/{id?}");

app.Run();