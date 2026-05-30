namespace TasteDiary;

/// <summary>
/// Application Shell defining the navigation structure of the app.
/// Uses a three-tab layout (Foods, Hardware, Settings) with two additional routes
/// (<see cref="AddItemPage"/> and <see cref="FoodDetailPage"/>) pushed on the navigation stack.
/// </summary>
public partial class AppShell : Shell
{
    /// <summary>
    /// Initialises the Shell XAML and registers routes for pages that are navigated to
    /// via <c>Shell.Current.GoToAsync</c> rather than being fixed in the tab bar.
    /// </summary>
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute(nameof(AddItemPage), typeof(AddItemPage));
        Routing.RegisterRoute(nameof(FoodDetailPage), typeof(FoodDetailPage));
    }
}
