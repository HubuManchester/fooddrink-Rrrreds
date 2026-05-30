namespace TasteDiary.Services;

/// <summary>
/// Wraps the platform Text-to-Speech API with English locale detection and cancellation support.
/// Used on the Hardware page and Food Detail page to read nutrition summaries aloud.
/// </summary>
public static class SpeechService
{
    private static CancellationTokenSource? currentSpeech;

    /// <summary>
    /// Stops any ongoing speech, then reads the provided text aloud in English at 0.9 volume and 1.05 pitch.
    /// </summary>
    /// <param name="text">The text content to read aloud.</param>
    public static async Task SpeakAsync(string text)
    {
        Stop();

        currentSpeech = new CancellationTokenSource();
        var options = new SpeechOptions
        {
            Volume = 0.9f,
            Pitch = 1.05f,
            Locale = await FindEnglishLocaleAsync()
        };

        try
        {
            await TextToSpeech.Default.SpeakAsync(text, options, currentSpeech.Token);
        }
        catch (OperationCanceledException)
        {
        }
    }

    /// <summary>
    /// Convenience alias for <see cref="SpeakAsync"/> when Chinese text is passed.
    /// </summary>
    /// <param name="text">The text content to read aloud.</param>
    public static Task SpeakChineseAsync(string text) => SpeakAsync(text);

    /// <summary>
    /// Cancels any currently-running speech and releases the cancellation token.
    /// </summary>
    public static void Stop()
    {
        if (currentSpeech is null)
        {
            return;
        }

        currentSpeech.Cancel();
        currentSpeech.Dispose();
        currentSpeech = null;
    }

    /// <summary>
    /// Searches the device's available locales and returns the first one whose language starts with "en".
    /// </summary>
    /// <returns>An English <see cref="Locale"/>, or <c>null</c> if none is available.</returns>
    private static async Task<Locale?> FindEnglishLocaleAsync()
    {
        var locales = await TextToSpeech.Default.GetLocalesAsync();
        return locales.FirstOrDefault(locale => locale.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase));
    }
}
