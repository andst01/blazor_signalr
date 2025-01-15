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
builder.Services.AddSignalR(hubOptions =>
{
    hubOptions.EnableDetailedErrors = true;
    hubOptions.KeepAliveInterval = TimeSpan.FromSeconds(1);
});
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

app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SeatHub>("/seathub", options =>
    {
        options.Transports =
           Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
           Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
    });
});

/*
app.MapHub<SeatHub>("/seathub", options =>
{
    options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                         Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;
});
*/

app.Run();
