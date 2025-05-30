using DotNetEnv;
using Server;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    Env.Load();
}

builder.Configuration.AddEnvironmentVariables();

var mongoConnString = builder.Configuration["MONGO_CONNECTION_STRING"];
var sendGridKey = builder.Configuration["SENDGRID_API_KEY"];


builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("policy",
        policy =>
        {;
            policy.AllowAnyOrigin();
            policy.AllowAnyMethod();
            policy.AllowAnyHeader();
        });
});


builder.Services.AddSingleton<IUserRepository,UserRepository>();
builder.Services.AddSingleton<ITemplateRepository,TemplateRepository>();
builder.Services.AddSingleton<IElevplanRepository,ElevplanRepositoryRepository>();
builder.Services.AddSingleton<IGoalRepository,GoalRepository>();
builder.Services.AddSingleton<IHotelRepository, HotelRepository>();
builder.Services.AddSingleton<IKursusRepository, KursusRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});



builder.Services.AddOpenApi();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapOpenApi();

app.UseRouting();

app.UseHttpsRedirection();
app.UseCors("policy");
app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles(); 

app.MapFallbackToFile("index.html");

app.Run();
