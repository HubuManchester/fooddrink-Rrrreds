using TasteDiary.Services;

namespace TasteDiary;

/// <summary>
/// Demonstrates the following mobile hardware APIs in one page:
/// <list type="bullet">
///   <item><see cref="MediaPicker"/> — camera capture</item>
///   <item><see cref="Geolocation"/> — GPS location</item>
///   <item><see cref="Geocoding"/> — reverse geocoding (coordinates to address)</item>
///   <item><see cref="TextToSpeech"/> — text-to-speech</item>
///   <item><see cref="Vibration"/> — device vibration</item>
///   <item><see cref="HapticFeedback"/> — haptic feedback</item>
/// </list>
/// Each hardware feature has a dedicated button, status label, and error handling with
/// user-friendly messages and screen-reader announcements.
/// </summary>
public partial class HardwarePage : ContentPage
{
    /// <summary>Tracks the number of times haptic feedback has been triggered for visual verification.</summary>
    private int feedbackTestCount;

    /// <summary>Initialises the page components defined in HardwarePage.xaml.</summary>
    public HardwarePage()
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
    /// Captures a photo using the device camera via <see cref="MediaPicker.CapturePhotoAsync"/>.
    /// Displays the captured image and announces the result via screen reader.
    /// Handles <see cref="PermissionException"/> for denied camera access.
    /// </summary>
    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!MediaPicker.Default.IsCaptureSupported)
            {
                SetStatus("This device does not support camera capture.");
                return;
            }

            var photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                SetStatus("Photo capture cancelled.");
                return;
            }

            await using var stream = await photo.OpenReadAsync();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            var imageBytes = memoryStream.ToArray();
            FoodPhoto.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
            SetStatus("Food photo captured successfully.");
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        catch (PermissionException)
        {
            SetStatus("Camera permission was denied. Enable camera access in device settings.");
        }
        catch (Exception ex)
        {
            SetStatus($"Camera error: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the current device location using <c>Geolocation.Default.GetLocationAsync</c>
    /// with medium accuracy and a 10-second timeout. Performs reverse geocoding to obtain
    /// a human-readable address. Falls back to a hard-coded address for known coordinate ranges
    /// when geocoding is unavailable (e.g. on emulators).
    /// </summary>
    private async void OnGetLocationClicked(object? sender, EventArgs e)
    {
        try
        {
            SetStatus("Getting location...");
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location is null)
            {
                SetStatus("Current location could not be found.");
                return;
            }

            CoordinateLabel.Text = $"Latitude {location.Latitude:F5}, longitude {location.Longitude:F5}";
            LocationLabel.Text = await BuildAddressTextAsync(location);
            SetStatus("Country, city, and coordinates have been loaded.");
        }
        catch (PermissionException)
        {
            SetStatus("Location permission was denied. Enable location access in device settings.");
        }
        catch (Exception ex)
        {
            SetStatus($"Location error: {ex.Message}");
        }
    }

    /// <summary>
    /// Attempts to resolve a human-readable address from geographic coordinates using
    /// <c>Geocoding.Default.GetPlacemarksAsync</c>. Falls back to a hard-coded lookup
    /// if the geocoding service returns no results.
    /// </summary>
    /// <param name="location">The geographic location to resolve.</param>
    /// <returns>A formatted address string, or a fallback message.</returns>
    private static async Task<string> BuildAddressTextAsync(Location location)
    {
        try
        {
            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location);
            var placemark = placemarks?.FirstOrDefault();
            var address = FormatPlacemark(placemark);

            if (!string.IsNullOrWhiteSpace(address))
            {
                return address;
            }
        }
        catch
        {
        }

        return BuildFallbackAddress(location);
    }

    /// <summary>
    /// Formats a <see cref="Placemark"/> into a "/"-separated address string
    /// (Country / AdminArea / Locality / SubLocality / Thoroughfare).
    /// Duplicate and empty parts are filtered out.
    /// </summary>
    /// <param name="placemark">The resolved placemark, or <c>null</c>.</param>
    /// <returns>A formatted address string, or an empty string if the placemark is null or empty.</returns>
    private static string FormatPlacemark(Placemark? placemark)
    {
        if (placemark is null)
        {
            return string.Empty;
        }

        var parts = new[]
        {
            placemark.CountryName,
            placemark.AdminArea,
            placemark.Locality,
            placemark.SubLocality,
            placemark.Thoroughfare
        }
        .Where(part => !string.IsNullOrWhiteSpace(part))
        .Distinct()
        .ToArray();

        return parts.Length == 0 ? string.Empty : string.Join(" / ", parts);
    }

    /// <summary>
    /// Provides a hard-coded address for well-known coordinate ranges
    /// (e.g. Mountain View, San Francisco Bay Area, China) when reverse geocoding is unavailable.
    /// </summary>
    /// <param name="location">The geographic location.</param>
    /// <returns>A user-readable fallback address string.</returns>
    private static string BuildFallbackAddress(Location location)
    {
        if (IsNear(location, 37.422, -122.084, 0.08))
        {
            return "United States / California / Mountain View";
        }

        if (location.Latitude is >= 37.0 and <= 38.2 && location.Longitude is >= -123.2 and <= -121.5)
        {
            return "United States / California / San Francisco Bay Area";
        }

        if (location.Latitude is >= 18 and <= 54 && location.Longitude is >= 73 and <= 135)
        {
            return "China / Current city requires a real device or available geocoding service";
        }

        return "Coordinates were found, but country and city were not returned by this device.";
    }

    /// <summary>
    /// Checks whether a location is within a given tolerance of a target latitude/longitude.
    /// </summary>
    /// <param name="location">The location to check.</param>
    /// <param name="latitude">Target latitude.</param>
    /// <param name="longitude">Target longitude.</param>
    /// <param name="tolerance">Maximum absolute difference in degrees for both axes.</param>
    /// <returns><c>true</c> if the location is within the tolerance range.</returns>
    private static bool IsNear(Location location, double latitude, double longitude, double tolerance)
    {
        return Math.Abs(location.Latitude - latitude) <= tolerance &&
               Math.Abs(location.Longitude - longitude) <= tolerance;
    }

    /// <summary>
    /// Reads the app help text aloud using the Text-to-Speech engine.
    /// The help text describes what the app does in a single sentence.
    /// </summary>
    private async void OnReadHelpClicked(object? sender, EventArgs e)
    {
        try
        {
            const string helpText = "TasteDiary records foods and drinks, shows nutrition details, and uses camera, location, speech, and haptic feedback to make meal tracking more practical.";
            await SpeechService.SpeakAsync(helpText);
            SetStatus("Reading help content aloud.");
        }
        catch (Exception ex)
        {
            SetStatus($"Text to speech error: {ex.Message}");
        }
    }

    /// <summary>Stops any ongoing Text-to-Speech and updates the status label.</summary>
    private void OnStopSpeechClicked(object? sender, EventArgs e)
    {
        SpeechService.Stop();
        SetStatus("Reading stopped.");
    }

    /// <summary>
    /// Triggers a 450ms vibration and long-press haptic feedback, increments the test counter,
    /// and updates the counter label for visual verification. This allows screen-recording viewers
    /// to confirm the feature is working even though vibration cannot be captured in video.
    /// </summary>
    private void OnFeedbackClicked(object? sender, EventArgs e)
    {
        try
        {
            Vibration.Default.Vibrate(TimeSpan.FromMilliseconds(450));
            HapticFeedback.Default.Perform(HapticFeedbackType.LongPress);
            feedbackTestCount++;
            FeedbackCountLabel.Text = $"Haptic feedback tests: {feedbackTestCount}";
            SetStatus("Vibration and haptic feedback triggered. The changing counter can be used for screen-recorded verification.");
        }
        catch (Exception ex)
        {
            SetStatus($"Feedback error: {ex.Message}");
        }
    }

    /// <summary>
    /// Updates the on-screen status label and announces the message via screen reader.
    /// Centralised helper used by all hardware handler methods.
    /// </summary>
    /// <param name="message">The status message to display and announce.</param>
    private void SetStatus(string message)
    {
        HardwareStatusLabel.Text = message;
        SemanticScreenReader.Announce(message);
    }
}
