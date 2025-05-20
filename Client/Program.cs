using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

string apiBaseUrl = builder.Configuration["ApiBaseUrl"];

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseUrl) });

//Boostrap
builder.Services.AddBlazorBootstrap();

//Local Storage
builder.Services.AddBlazoredLocalStorage();

//Services
builder.Services.AddScoped<IAuth, AuthServiceServer>();
builder.Services.AddScoped<IBruger, BrugerServiceServer>();
builder.Services.AddScoped<IHotel, HotelServiceServer>();
builder.Services.AddScoped<IElevPlan, ElevPlanServiceServer>();
builder.Services.AddScoped<IGoal, GoalServiceServer>();
builder.Services.AddScoped<ITemplate, TempalteServiceServer>();
builder.Services.AddScoped<IKursus, KursusServiceServer>();

await builder.Build().RunAsync();