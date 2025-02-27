using Football_Predictions.Models;
using Football_Predictions.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add the MVC configuration for controllers and views
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient<FootballDataService>(); // Register FootballDataService with HttpClient


var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    // In production, use the error handler for exceptions
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is set to 30 days. You can change it for a production environment
    app.UseHsts();
}

// Use HTTPS redirection
app.UseHttpsRedirection();

// Serve static files like images, CSS, JavaScript, etc.
app.UseStaticFiles();

// Enable routing to map the controller actions
app.UseRouting();

// Enable authorization for secure endpoints
app.UseAuthorization();

// Map the default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Run the application
app.Run();
