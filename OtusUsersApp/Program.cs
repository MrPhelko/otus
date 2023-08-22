using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OtusUsersApp;
using OtusUsersApp.DB;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddOpenTelemetry()
//    .WithMetrics(builder =>
//    {
//        builder.AddPrometheusExporter();

//        builder.AddMeter("Microsoft.AspNetCore.Hosting",
//                         "Microsoft.AspNetCore.Server.Kestrel");
//        builder.AddView("http-server-request-duration",
//            new ExplicitBucketHistogramConfiguration
//            {
//                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
//                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 }
//            });
//    });

builder.Host.ConfigureAppConfiguration((hosting, config) =>
{
    var env = hosting.HostingEnvironment;
    config
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddJsonFile("configs/appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
    ;
});


Console.WriteLine(builder.Configuration.GetDebugView());

var connectionString = GetConnectionStrings(builder.Configuration);
// Add services to the container.
builder.Services.AddDbContext<UsersContext>(o => 
                    o.UseNpgsql(connectionString));
builder.Services.AddHealthChecks();

builder.Services.AddTransient<IJwtUtils, JwtUtils>();
builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
InitializeDatabase(app);
app.UseMiddleware<JwtMiddleware>();

app.MapHealthChecks("/health");
app.MapHealthChecks("/ready");
app.UseMetricServer();
app.MapControllers();

app.Run();

void InitializeDatabase(IApplicationBuilder app)
{
    using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
    {
        scope.ServiceProvider.GetRequiredService<UsersContext>().Database.Migrate();
    }
}

string GetConnectionStrings(IConfiguration configuration)
{
    var connectionString = configuration.GetConnectionString("Main");

    var dbPassword = configuration.GetValue<string>("DB_PASSWORD");
    var dbUserName = configuration.GetValue<string>("DB_USERNAME");

    return connectionString
        .Replace("{DB_PASSWORD}", dbPassword)
        .Replace("{DB_USERNAME}", dbUserName);
}