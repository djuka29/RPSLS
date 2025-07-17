using RPSLS.Application.Interfaces;
using RPSLS.Application.Services;
using RPSLS.Infrastructure.LoggingMiddleware;
using RPSLS.Infrastructure.Repositories;
using RPSLS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DI registration
builder.Services.AddHttpClient<IRandomNumberService, RandomNumberService>();
builder.Services.AddSingleton<IRPSLSGameService, RPSLSGameService>();
builder.Services.AddSingleton<IUserRepository>(
    new UserRepository("Data Source=users.db")
); 

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var logFilePath = Path.Combine(app.Environment.ContentRootPath, "logs", "requests.log");
Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
app.UseMiddleware<LoggingMiddleware>(logFilePath);

app.UseCors();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();