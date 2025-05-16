
using Client;
using Server;

var builder = WebApplication.CreateBuilder(args);



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
builder.Services.AddSingleton<IElevplan,ElevplanRepository>();
builder.Services.AddSingleton<IGoalRepository,GoalRepository>();
builder.Services.AddSingleton<IKursusRepository, KursusRepository>();


builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("policy");
app.UseAuthorization();

app.MapControllers();

app.Run();