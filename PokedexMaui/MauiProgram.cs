using Microsoft.Extensions.Logging;
using PokedexMaui.Services;

namespace PokedexMaui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddSingleton(new HttpClient
        {
            BaseAddress = new Uri("https://pokeapi.co/api/v2/"),
            Timeout = TimeSpan.FromSeconds(15)
        });

        builder.Services.AddSingleton<PokeApiService>();
        builder.Services.AddTransient<MainPage>();

        // Parte 13. El reto de vehiculos usa otra API, por lo que necesita su
        // propio HttpClient con una direccion base distinta.
        builder.Services.AddSingleton(_ => new VehicleApiService(new HttpClient
        {
            BaseAddress = new Uri("https://vpic.nhtsa.dot.gov/api/vehicles/"),
            Timeout = TimeSpan.FromSeconds(30)
        }));

        builder.Services.AddTransient<VehiclesPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
