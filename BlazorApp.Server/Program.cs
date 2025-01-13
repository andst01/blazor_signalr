using Blazor.Extensions;
using Blazor.Extensions.Canvas;
using BlazorApp.Server.Data;
using BlazorApp.Server.SignalRHub;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<BECanvasComponent>();
builder.Services.AddScoped<BECanvas>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<WeatherForecastService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.MapHub<SeatHub>("/seathub");

app.Run();
