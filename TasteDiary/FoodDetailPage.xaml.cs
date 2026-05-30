using TasteDiary.Models;
using TasteDiary.Services;

namespace TasteDiary;

/// <summary>
/// Displays detailed nutrition information for a single food item.
/// Receives the item ID via Shell query property (<c>?id=...</c>).
/// Supports Text-to-Speech for reading the nutrition summary aloud and vibration for haptic feedback.
/// </summary>
[QueryProperty(nameof(ItemId), "id")]
public partial class FoodDetailPage : ContentPage
{
    /// <summary>The currently loaded food item, or <c>null</c> if not yet loaded.</summary>
    private FoodItem? currentItem;

    /// <summary>Initialises the page components defined in FoodDetailPage.xaml.</summary>
    public FoodDetailPage()
    {
        InitializeComponent();
    }

    /// <summary>Applies accessibility font scaling when the page appears.</summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    /// <summary>Stops any ongoing Text-to-Speech when navigating away from this page.</summary>
    protected override void OnDisappearing()
    {
        SpeechService.Stop();
        base.OnDisappearing();
    }

    /// <summary>
    /// Shell query property setter. Receives the food item ID from navigation and triggers loading.
    /// </summary>
    public string ItemId
    {
        set => _ = LoadItemAsync(value);
    }

    /// <summary>
    /// Loads the food item by ID from the data service and renders it into the UI.
    /// If the item cannot be found, a "Record not found" message is displayed.
    /// </summary>
    /// <param name="id">The unique identifier of the food item to load.</param>
    private async Task LoadItemAsync(string id)
    {
        currentItem = await FoodCatalogService.GetByIdAsync(id);
        BindingContext = currentItem;
        RenderItem();
    }

    /// <summary>
    /// Manually sets all UI labels and the image source from the current item.
    /// Falls back to a "Record not found" message if no item is loaded.
    /// </summary>
    private void RenderItem()
    {
        if (currentItem is null)
        {
            NameLabel.Text = "Record not found";
            DescriptionLabel.Text = "The selected food or drink could not be loaded.";
            return;
        }

        NameLabel.Text = currentItem.Name;
        CategoryLabel.Text = currentItem.Category;
        CaloriesLabel.Text = currentItem.CaloriesLabel;
        MacroLabel.Text = currentItem.MacroSummary;
        DescriptionLabel.Text = currentItem.Description;
        AllergyLabel.Text = currentItem.AllergyNote;
        ItemImage.Source = currentItem.ItemImage;
        SemanticProperties.SetDescription(NameLabel, currentItem.AccessibleSummary);
    }

    /// <summary>
    /// Reads the current item's nutrition summary aloud using the device Text-to-Speech engine.
    /// Shows an alert if no item is loaded or if TTS is unavailable.
    /// </summary>
    private async void OnSpeakClicked(object? sender, EventArgs e)
    {
        if (currentItem is null)
        {
            await DisplayAlert("Missing record", "There is no nutrition summary to read.", "OK");
            return;
        }

        try
        {
            await SpeechService.SpeakAsync(currentItem.AccessibleSummary);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Text to speech unavailable", ex.Message, "OK");
        }
    }

    /// <summary>Stops any ongoing Text-to-Speech and announces the action via screen reader.</summary>
    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SemanticScreenReader.Announce("Reading stopped.");
    }

    /// <summary>
    /// Triggers a 500ms vibration and haptic long-press feedback, then shows a confirmation alert.
    /// Demonstrates the device vibration and haptic feedback hardware APIs.
    /// </summary>
    private async void OnVibrateClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(500));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            await DisplayAlert("Reminder", "Vibration feedback has been triggered.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Vibration unavailable", ex.Message, "OK");
        }
    }
}
