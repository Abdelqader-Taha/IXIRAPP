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

// ?? Configure Logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

// ?? Improved CORS Handling (Allows Authorization header for JWT)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
    policy => policy
        .WithOrigins("http://localhost:3000") // Allow frontend domain
        .AllowAnyMethod()
        .AllowAnyHeader()
        .WithExposedHeaders("Authorization") // Allow JWT tokens
        .AllowCredentials());
});

// ?? Add Controllers with JSON Configuration
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
        options.SerializerSettings.Converters.Add(new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal });
    });

builder.Services.AddEndpointsApiExplorer();

// ?? Swagger Configuration (Default Theme)
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

// ?? Database Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<DataContext>();
    DataContext.SeedAdminUser(context, "admin", "admin");
}

// ?? Correct Middleware Ordering
app.UseCors("AllowAllOrigins"); // ? Apply CORS before authentication
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.UseMiddleware<CustomPayloadTooLargeMiddleware>();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(
    options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "IXIR API v1");

        // Inject custom CSS for Dark Mode
        options.InjectJavascript("/swagger-ui/custom.js");

        options.InjectStylesheet("/swagger-ui/custom.css");
    }



    );

app.MapControllers();

app.Run();
