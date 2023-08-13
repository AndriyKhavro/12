using RedisTest.Library;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var serviceName = configuration["Redis:ServiceName"]!;
var host = configuration["Redis:Host"]!;
var port = int.Parse(configuration["Redis:Port"]!);
var password = configuration["Redis:Password"]!;

Console.WriteLine("Registering RedisClient");

builder.Services.AddSingleton(_ => new RedisClient(serviceName, host, port, password));
builder.Services.AddHttpClient();
builder.Services.AddLogging(log => log.SetMinimumLevel(LogLevel.Warning));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();

app.Run();
