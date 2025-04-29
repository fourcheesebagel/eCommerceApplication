using eCommerceApp.Infrastructure.DependencyInjection;
using eCommerceApp.Application.DependencyInjection;
using Serilog;

//THIS IS OUR Presentation Layer

var builder = WebApplication.CreateBuilder(args);

//This Logger here will create a folder, into a file called log.txt, logging info daily once this is running, so whenever this runs continuously it will daily log to this file
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("log/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();
Log.Logger.Information("Application is Building.....");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructureService(builder.Configuration); //Contains our DB Connection
builder.Services.AddApplicationService();
builder.Services.AddCors(builder => //Security controls for which websites or apps are allowed to talk to your server
{
    builder.AddDefaultPolicy(options =>
    {
        options.AllowAnyHeader() //allows any kind of data (JSON,text etc., to be sent in requests)
        .AllowAnyMethod() //Allows any HTTP Method (GET, PUT DELETE, POST)
        .AllowAnyOrigin() //allows users from any port can access the methods for the HTTP calls, allows any website or app, to call the server
        .AllowCredentials(); //ALLOWS cookies and auth tokens to be sent with requests
    });
});

try
{
    var app = builder.Build();
    app.UseCors();
    app.UseSerilogRequestLogging();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseInfrastructureService();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    Log.Logger.Information("Application is running........");

    app.Run();
} catch (Exception ex)
{
    Log.Logger.Error(ex, "Application failed to start");
} finally
{
    Log.CloseAndFlush();
}