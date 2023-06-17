using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json.Serialization;
using ChrisUsher.MoveMate.API.Database;
using ChrisUsher.MoveMate.API.Repositories;
using ChrisUsher.MoveMate.API.Services.Accounts;
using ChrisUsher.MoveMate.API.Services.Mortgages;
using ChrisUsher.MoveMate.API.Services.Savings;
using ChrisUsher.MoveMate.API.Services.StampDuty;
using ChrisUsher.MoveMate.API.Services.Properties;

var config = new ConfigurationBuilder()
#if DEBUG
    .AddJsonFile("local.settings.json", false)
#endif
    .AddEnvironmentVariables()
    .Build();

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureOpenApi()
    .ConfigureServices(services =>
    {
        services.AddDbContext<DatabaseContext>(options => 
        {
            options.UseCosmos(config["Database:AccountName"], config["Database:Key"], config["Database:DatabaseName"]);

            options.EnableSensitiveDataLogging();

            #if DEBUG
            
            options.EnableDetailedErrors();
            options.LogTo(Console.WriteLine);

            #endif
        });

        services.ConfigureHttpJsonOptions(options => 
        {
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.SerializerOptions.WriteIndented = true;
        });

        services.AddSingleton<StampDutyService>();
        services.AddSingleton<AccountRepository>();
        services.AddSingleton<AccountService>();
        services.AddSingleton<SavingsRepository>();
        services.AddSingleton<SavingsService>();
        services.AddSingleton<PropertyRepository>();
        services.AddSingleton<PropertyService>();
        services.AddSingleton<InterestService>();
        services.AddSingleton<MortgagePaymentService>();
    })
    .Build();

    var dbContext = host.Services.GetRequiredService<DatabaseContext>();
    await dbContext.Database.EnsureCreatedAsync();

host.Run();
