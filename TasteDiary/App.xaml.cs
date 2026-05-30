namespace TasteDiary;

/// <summary>
/// Application entry point. Creates the main window with <see cref="AppShell"/>
/// as the root page, which provides the tab bar and route-based navigation.
/// </summary>
public partial class App : Application
{
    /// <summary>Initialises the application XAML resources (styles, colours, etc.).</summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Creates the application's main window, hosting the <see cref="AppShell"/> navigation structure.
    /// </summary>
    /// <param name="activationState">The current activation state (not used directly).</param>
    /// <returns>A new <see cref="Window"/> containing the app shell.</returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        return new Window(new AppShell());
    }
}
