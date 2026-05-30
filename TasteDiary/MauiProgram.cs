using Microsoft.Extensions.Logging;

namespace TasteDiary;

/// <summary>
/// .NET MAUI program entry point. Configures the app builder with fonts,
/// debug logging (in DEBUG builds), and the <see cref="App"/> class as the root.
/// </summary>
public static class MauiProgram
{
    /// <summary>
    /// Creates and configures the <see cref="MauiApp"/> instance.
    /// Registers custom fonts (OpenSans Regular and Semibold) and enables
    /// debug-level logging when building in Debug configuration.
    /// </summary>
    /// <returns>The built <see cref="MauiApp"/> ready to run.</returns>
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

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
