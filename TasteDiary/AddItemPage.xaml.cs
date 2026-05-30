using TasteDiary.Models;
using TasteDiary.Services;

namespace TasteDiary;

/// <summary>
/// Form page for adding a new food or drink record. Provides validated text entry for
/// name, category, description, calories, protein, carbs, fat, and allergen notes.
/// On successful save the record is persisted via <see cref="FoodCatalogService.AddAsync"/>
/// and the user is returned to the main list.
/// </summary>
public partial class AddItemPage : ContentPage
{
    /// <summary>Initialises the page components defined in AddItemPage.xaml.</summary>
    public AddItemPage()
    {
        InitializeComponent();
    }

    /// <summary>Applies accessibility font scaling when the page appears.</summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        AccessibilityService.ApplyFontScale(this);
    }

    /// <summary>
    /// Validates all form fields, constructs a <see cref="FoodItem"/>, saves it,
    /// provides haptic feedback, and navigates back to the main page.
    /// Shows a user-friendly validation message if any field is invalid.
    /// </summary>
    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        try
        {
            var validationMessage = ValidateForm(out var calories, out var protein, out var carbs, out var fat);
            if (validationMessage is not null)
            {
                ShowValidation(validationMessage);
                Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(250));
                return;
            }

            var item = new FoodItem
            {
                Name = NameEntry.Text!.Trim(),
                Category = CategoryPicker.SelectedItem?.ToString() ?? "Snack",
                Description = DescriptionEditor.Text!.Trim(),
                Calories = calories,
                Protein = protein,
                Carbs = carbs,
                Fat = fat,
                AllergyNote = string.IsNullOrWhiteSpace(AllergyEntry.Text)
                    ? "No allergy note provided."
                    : AllergyEntry.Text.Trim(),
                Tags = $"{NameEntry.Text} {CategoryPicker.SelectedItem} {DescriptionEditor.Text}"
            };

            await FoodCatalogService.AddAsync(item);
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
            SemanticScreenReader.Announce("Food record saved.");

            await DisplayAlert(
                "Saved",
                MockApiConfig.IsConfigured
                    ? "The record has been saved to mockapi.io."
                    : "The record has been saved to local fallback data.",
                "OK");

            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            ShowValidation($"The record could not be saved: {ex.Message}");
        }
    }

    /// <summary>
    /// Validates all required form fields. Returns an error message string if validation fails,
    /// or <c>null</c> if all fields are valid. Outputs parsed numeric values via <c>out</c> parameters.
    /// </summary>
    /// <param name="calories">Parsed calorie value.</param>
    /// <param name="protein">Parsed protein value in grams.</param>
    /// <param name="carbs">Parsed carbohydrate value in grams.</param>
    /// <param name="fat">Parsed fat value in grams.</param>
    /// <returns>A user-friendly error message if validation fails; otherwise <c>null</c>.</returns>
    private string? ValidateForm(out int calories, out int protein, out int carbs, out int fat)
    {
        calories = protein = carbs = fat = 0;

        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            return "Please enter a food or drink name.";
        }

        if (CategoryPicker.SelectedIndex < 0)
        {
            return "Please choose a category.";
        }

        if (string.IsNullOrWhiteSpace(DescriptionEditor.Text))
        {
            return "Please add a short description.";
        }

        return TryReadNumber(CaloriesEntry.Text, "calories", out calories)
            ?? TryReadNumber(ProteinEntry.Text, "protein", out protein)
            ?? TryReadNumber(CarbsEntry.Text, "carbs", out carbs)
            ?? TryReadNumber(FatEntry.Text, "fat", out fat);
    }

    /// <summary>
    /// Attempts to parse a non-negative integer from a string value.
    /// Returns an error message if parsing fails or the number is negative.
    /// </summary>
    /// <param name="value">The string to parse.</param>
    /// <param name="fieldName">The display name of the field, used in the error message.</param>
    /// <param name="number">The parsed integer value.</param>
    /// <returns>An error message if invalid; otherwise <c>null</c>.</returns>
    private static string? TryReadNumber(string? value, string fieldName, out int number)
    {
        if (int.TryParse(value, out number) && number >= 0)
        {
            return null;
        }

        return $"Please enter a valid non-negative number for {fieldName}.";
    }

    /// <summary>
    /// Displays a validation error message in the validation panel and announces it via screen reader.
    /// </summary>
    /// <param name="message">The user-friendly error message to display.</param>
    private void ShowValidation(string message)
    {
        ValidationLabel.Text = message;
        ValidationPanel.IsVisible = true;
        SemanticScreenReader.Announce(message);
    }
}
