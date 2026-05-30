using Foundation;

namespace TasteDiary;

/// <summary>
/// iOS/macOS application delegate. Serves as the native entry point for the MAUI app.
/// Registers with the <c>AppDelegate</c> Objective-C selector.
/// </summary>
[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
    /// <summary>Creates the MAUI application instance.</summary>
    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
