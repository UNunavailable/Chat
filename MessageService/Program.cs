using MessageService.Models;
using MessageService.SignalR;

var builder = WebApplication.CreateBuilder(args);
var logger = LoggerFactory
    .Create(config =>
    {
        config.AddConsole();
    }).CreateLogger("Program");

// Services
logger.LogInformation("Adding services...");
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// CORS
logger.LogInformation("Trying to get CORS parameters...");
var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"]?.Split(';')
    ?? [];
logger.LogInformation("Got parameters. Adding CORS...");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClients", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Database
logger.LogInformation("Trying to get database parameters...");
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new NullReferenceException("Specify your database connection string: DefaultConnection");
logger.LogInformation("Got parameters. Adding database repository...");
builder.Services.AddSingleton(new MessageRepository(
    connectionString,
    logger: builder.Services.BuildServiceProvider().GetRequiredService<ILogger<MessageRepository>>()));

logger.LogInformation("Building application...");
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseCors("AllowClients");

app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");

logger.LogInformation("Running application...");
app.Run();
