using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeightAnalysis.Components;
using WeightAnalysis.Configurations;
using WeightAnalysis.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddScoped<HttpClient>();

builder.Services.AddSwaggerGen();


var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
var withingsConfig = config.GetSection("Withings").Get<WithingsConfiguration>();
builder.Services.AddSingleton<WithingsConfiguration>(withingsConfig);

builder.Services.AddDbContextFactory<WeightContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
