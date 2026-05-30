using TasteDiary.Services;

namespace TasteDiary;

/// <summary>
/// Main food list page. Displays all food and drink items in a searchable, pull-to-refresh list.
/// Each item card shows a thumbnail, name, calorie badge, description, macro summary, category,
/// and a "Details" button that navigates to the nutrition detail page.
/// The "Add" button in the header navigates to <see cref="AddItemPage"/>.
/// </summary>
public partial class MainPage : ContentPage
{
    /// <summary>Initialises the page components defined in MainPage.xaml.</summary>
    public MainPage()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Re-applies the accessibility font scale and reloads the food list every time the page appears,
    /// ensuring the data is fresh and the UI respects the current accessibility settings.
    /// </summary>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    /// <summary>
    /// Fetches food items from the data service and binds them to the CollectionView.
    /// </summary>
    /// <param name="query">Optional search query to filter results.</param>
    private async Task LoadFoodItemsAsync(string? query = null)
    {
        FoodCollection.ItemsSource = await FoodCatalogService.SearchAsync(query);
    }

    /// <summary>Navigates to the Add Item form page.</summary>
    private async void OnAddClicked(object? sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddItemPage));
    }

    /// <summary>
    /// Navigates to the food detail page, passing the selected item's ID as a query parameter.
    /// </summary>
    private async void OnDetailsClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is string id)
        {
            await Shell.Current.GoToAsync($"{nameof(FoodDetailPage)}?id={Uri.EscapeDataString(id)}");
        }
    }

    /// <summary>Filters the food list in real time as the user types in the search bar.</summary>
    private async void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        await LoadFoodItemsAsync(e.NewTextValue);
    }

    /// <summary>Executes the search when the user presses the search button on the keyboard.</summary>
    private async void OnSearchButtonPressed(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
    }

    /// <summary>
    /// Handles the pull-to-refresh gesture. Reloads the food list and announces
    /// the data source to the screen reader so users are aware of the current backend.
    /// </summary>
    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await LoadFoodItemsAsync(SearchFoodBar.Text);
        FoodRefreshView.IsRefreshing = false;
        var source = FoodCatalogService.LastLoadUsedMockApi ? "mockapi.io" : "local fallback data";
        SemanticScreenReader.Announce($"Food and drink list refreshed. Current source: {source}.");
    }
}
