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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
