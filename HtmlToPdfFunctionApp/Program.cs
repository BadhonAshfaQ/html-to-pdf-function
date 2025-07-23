using HtmlToPdfFunctionApp.Utility;

namespace HtmlToPdfFunctionApp;

public static class Program
{
    public static void Main()
    {
        var host = new HostBuilder()
            .ConfigureFunctionsWebApplication()
            .ConfigureServices(services =>
            {
                services.AddApplicationInsightsTelemetryWorkerService();
                services.ConfigureFunctionsApplicationInsights();
                services.AddMemoryCache();
               
            })
            .Build();

        host.Run();
    }

    public static IConfiguration Configuration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", ""))
            .AddJsonFile("global.settings.json", optional: false, reloadOnChange: true);

        var conf = builder.Build();
        builder.AddAzureKeyVault(new Uri(Encryption.Decrypt(conf["0f3abba00164:a8d87a6cfdb6"])), new ClientSecretCredential(
            Encryption.Decrypt(conf["0f3abba00164:d180e39c390d"]),
            Encryption.Decrypt(conf["0f3abba00164:b626a4a69493"]),
            Encryption.Decrypt(conf["0f3abba00164:c974526b1ef5"])));

        return builder.Build();
    }
}