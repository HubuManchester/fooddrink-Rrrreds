namespace TasteDiary.Services;

/// <summary>
/// Configuration for optional mockapi.io integration.
/// When <see cref="EndpointUrl"/> is set to a valid mockapi.io resource endpoint,
/// the app will read from and write to the remote API instead of using local fallback data.
/// </summary>
public static class MockApiConfig
{
    /// <summary>
    /// The mockapi.io resource endpoint URL.
    /// Replace this with your generated endpoint (e.g. "https://682xxxx.mockapi.io/api/v1/foods").
    /// Leave empty to use local fallback data.
    /// </summary>
    public const string EndpointUrl = "";

    /// <summary>
    /// Returns <c>true</c> when a valid mockapi.io endpoint has been configured.
    /// </summary>
    public static bool IsConfigured => !string.IsNullOrWhiteSpace(EndpointUrl);
}
