using System.Globalization;
using EvaluationBackend.DATA;
using EvaluationBackend.Extensions;
using EvaluationBackend.Helpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;
using ConfigurationProvider = EvaluationBackend.Helpers.ConfigurationProvider;

var builder = WebApplication.CreateBuilder(args);


Log.Logger = new LoggerConfiguration()
.MinimumLevel.Error()
.WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
.CreateLogger();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
    builder => builder
    .AllowAnyOrigin()
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithExposedHeaders("status")); 
});

// Add services to the container.

builder.Services.AddControllers()
.AddNewtonsoftJson(options =>
{
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
});;
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();



builder.Services.AddSwaggerGen(options =>
{
    options.OperationFilter<PascalCaseQueryParameterFilter>();
});
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
IConfiguration configuration = builder.Configuration;
ConfigurationProvider.Configuration = configuration;
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    DataContext.SeedAdminUser(context, "admin", "admin");  // Call with correct arguments
}


// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAllOrigins");

app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.UseMiddleware<CustomPayloadTooLargeMiddleware>();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.MapControllers();

app.Run();