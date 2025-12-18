using Avalonia;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TextAnonymizer;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppHost.Start(args);
        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}

internal static class AppHost
{
    public static IHost Host { get; private set; } = null!;

    public static void Start(string[] args)
    {
        Host = new HostBuilder()
            .ConfigureDefaults(args)
            .ConfigureAppConfiguration((ctx, cfg) =>
            {
                cfg.SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                   .AddEnvironmentVariables();
            })
            .ConfigureServices((ctx, services) =>
            {
                services.Configure<OllamaSettings>(ctx.Configuration.GetSection("Ollama"));

                services.AddSingleton(sp =>
                {
                    var settings = sp.GetRequiredService<IOptions<OllamaSettings>>().Value;
                    var kb = Kernel.CreateBuilder();
                    kb.AddOpenAIChatCompletion(
                        modelId: settings.ModelName,
                        apiKey: "ollama",
                        endpoint: new Uri(settings.Url)
                    );
                    return kb.Build();
                });

                services.AddSingleton<OllamaService>();
                services.AddSingleton<MainWindow>();
            })
            .Build();

        Host.Start();
    }

    public static async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (Host == null) return;

        try
        {
            await Host.StopAsync(cancellationToken);
        }
        finally
        {
            Host = null!;
        }
    }
}
