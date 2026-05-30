using TasteDiary.Services;

namespace TasteDiary;

/// <summary>
/// Settings page providing theme switching and large-text accessibility toggle.
/// <list type="bullet">
///   <item><b>Theme picker:</b> System default, Light, or Dark — applied instantly via <see cref="Application.UserAppTheme"/>.</item>
///   <item><b>Large text switch:</b> Toggles <see cref="AccessibilityService.LargeTextEnabled"/> and re-applies the font scale to the current page. A preview area shows the effect visually.</item>
/// </list>
/// All changes are announced via screen reader and the status label.
/// </summary>
public partial class SettingsPage : ContentPage
{
    /// <summary>
    /// Initialises the page components defined in SettingsPage.xaml.
    /// Sets the theme picker to its default (System) and syncs the large-text switch
    /// with the current accessibility state.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        ThemePicker.SelectedIndex = 0;
        LargeTextSwitch.IsToggled = AccessibilityService.LargeTextEnabled;
    }

    /// <summary>
    /// Re-syncs the large-text switch with the accessibility service state and re-applies font scaling.
    /// Ensures consistency when returning to this page after changing settings on other pages.
    /// </summary>
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LargeTextSwitch.IsToggled = AccessibilityService.LargeTextEnabled;
        ApplyLargeTextState();
    }

    /// <summary>
    /// Applies the selected theme to the entire application immediately.
    /// Index 0 = System default, 1 = Light, 2 = Dark.
    /// </summary>
    private void OnThemeChanged(object? sender, EventArgs e)
    {
        Application.Current!.UserAppTheme = ThemePicker.SelectedIndex switch
        {
            1 => AppTheme.Light,
            2 => AppTheme.Dark,
            _ => AppTheme.Unspecified
        };

        Announce("Theme preference updated.");
    }

    /// <summary>
    /// Toggles the large-text accessibility mode on or off and re-applies font scaling
    /// to the current page, providing an immediate visual preview.
    /// </summary>
    private void OnLargeTextToggled(object? sender, ToggledEventArgs e)
    {
        AccessibilityService.LargeTextEnabled = e.Value;
        ApplyLargeTextState();
        Announce(e.Value
            ? "Large text mode is on. Page text is now larger."
            : "Large text mode is off. Page text has returned to normal.");
    }

    /// <summary>
    /// Re-applies font scaling to the entire visual tree and updates the preview labels
    /// to reflect the current large-text state.
    /// </summary>
    private void ApplyLargeTextState()
    {
        AccessibilityService.ApplyFontScale(this);

        LargeTextPreviewTitle.Text = AccessibilityService.LargeTextEnabled
            ? "Large text preview: enlarged"
            : "Large text preview";
        LargeTextPreviewBody.Text = AccessibilityService.LargeTextEnabled
            ? "Text is now noticeably larger. The food and hardware pages will use the same setting."
            : "Turn on the switch to enlarge this preview and other page text.";
    }

    /// <summary>
    /// Updates the status label and announces the message via the screen reader.
    /// Centralised helper for all settings change handlers.
    /// </summary>
    /// <param name="message">The message to display and announce.</param>
    private void Announce(string message)
    {
        SettingsStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}
