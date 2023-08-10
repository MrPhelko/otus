using Microsoft.EntityFrameworkCore;
using OtusUsersApp.DB;

var builder = WebApplication.CreateBuilder(args);


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

builder.Services.AddControllers();

var app = builder.Build();
// Configure the HTTP request pipeline.
InitializeDatabase(app);
app.UseAuthorization();

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