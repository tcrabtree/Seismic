using Microsoft.EntityFrameworkCore;
using Seismic.Data.Context;
using Seismic.Data.Interfaces;
using Seismic.Data.Repositories;
using Seismic.Data.Seeding;
using Seismic.UI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<ISeismicDataService, MockSeismicDataService>();
builder.Services.AddScoped<EventCsvParsingService>();

builder.Services.AddDbContext<SeismicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEventRepository, EventRepository>();
builder.Services.AddScoped<IWaveformRepository, WaveformRepository>();
builder.Services.AddScoped<IMonitorHealthRepository, MonitorHealthRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    await DatabaseSeeder.SeedAsync(app.Services);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Sites}/{action=Index}/{id?}");

app.Run();
