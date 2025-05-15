using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Client;
using Blazored.LocalStorage;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//Boostrap
builder.Services.AddBlazorBootstrap();

//Local Storage
builder.Services.AddBlazoredLocalStorage();

//Services
builder.Services.AddScoped<IAuth, AuthServiceMock>();
builder.Services.AddScoped<IBruger, BrugerServiceServer>();
builder.Services.AddScoped<IHotel, HotelServiceMock>();
builder.Services.AddScoped<IElevPlan, ElevPlanServiceServer>();
builder.Services.AddScoped<IGoal, GoalServiceMock>();
builder.Services.AddScoped<ITemplate, TempalteServiceServer>();
builder.Services.AddScoped<IKursus, KursusServiceMock>();

await builder.Build().RunAsync();