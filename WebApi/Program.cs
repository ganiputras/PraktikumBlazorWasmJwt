

using Serilog;
using WebApi;

Log.Logger = new LoggerConfiguration()
   .WriteTo.Console()
   .CreateBootstrapLogger();
Log.Information($"Architecture API starting");


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration
   .WriteTo.Console()
   .ReadFrom.Configuration(context.Configuration));

//var app = builder.Build();
var app =  builder
   .ConfigureServices()
   .ConfigurePipeline();


app.UseSerilogRequestLogging();
await app.ResetDatabaseAsync();



app.Run();
