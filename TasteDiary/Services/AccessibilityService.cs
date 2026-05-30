using System.Runtime.CompilerServices;

namespace TasteDiary.Services;

/// <summary>
/// Manages the large-text accessibility setting across all pages.
/// When enabled, all Label, Button, Entry, Editor, Picker, and SearchBar controls
/// are scaled by a factor of 1.22. Original font sizes are stored with
/// <see cref="ConditionalWeakTable{TKey,TValue}"/> so scaling is idempotent.
/// </summary>
public static class AccessibilityService
{
    /// <summary>Font scale factor applied when large text mode is enabled.</summary>
    private const double LargeTextScale = 1.22;

    /// <summary>Stores original font sizes per control to support idempotent scaling.</summary>
    private static readonly ConditionalWeakTable<BindableObject, FontSizeStore> OriginalFontSizes = new();

    /// <summary>
    /// Gets or sets whether large text mode is active. Persists across page navigations.
    /// </summary>
    public static bool LargeTextEnabled { get; set; }

    /// <summary>
    /// Recursively applies the current font scale to every supported control in the visual tree starting from <paramref name="root"/>.
    /// Call from each page's <c>OnAppearing</c> override.
    /// </summary>
    /// <param name="root">The root <see cref="Element"/> of the visual tree to process.</param>
    public static void ApplyFontScale(Element root)
    {
        ApplyToElement(root);

        if (root is not IVisualTreeElement visualTreeElement)
        {
            return;
        }

        foreach (var child in visualTreeElement.GetVisualChildren().OfType<Element>())
        {
            ApplyFontScale(child);
        }
    }

    /// <summary>
    /// Applies the current font scale to a single element based on its type.
    /// </summary>
    private static void ApplyToElement(Element element)
    {
        var scale = LargeTextEnabled ? LargeTextScale : 1.0;

        switch (element)
        {
            case Label label:
                label.FontSize = GetOriginalFontSize(label, label.FontSize) * scale;
                break;
            case Button button:
                button.FontSize = GetOriginalFontSize(button, button.FontSize) * scale;
                break;
            case Entry entry:
                entry.FontSize = GetOriginalFontSize(entry, entry.FontSize) * scale;
                break;
            case Editor editor:
                editor.FontSize = GetOriginalFontSize(editor, editor.FontSize) * scale;
                break;
            case Picker picker:
                picker.FontSize = GetOriginalFontSize(picker, picker.FontSize) * scale;
                break;
            case SearchBar searchBar:
                searchBar.FontSize = GetOriginalFontSize(searchBar, searchBar.FontSize) * scale;
                break;
        }
    }

    /// <summary>
    /// Retrieves the original font size for a control. On first call, stores the current size;
    /// on subsequent calls returns the stored value, enabling idempotent scaling.
    /// </summary>
    /// <param name="control">The bindable control.</param>
    /// <param name="currentSize">The current font size of the control.</param>
    /// <returns>The original font size before any scaling was applied.</returns>
    private static double GetOriginalFontSize(BindableObject control, double currentSize)
    {
        var store = OriginalFontSizes.GetOrCreateValue(control);
        if (!store.HasValue)
        {
            store.Value = currentSize > 0 ? currentSize : 14;
            store.HasValue = true;
        }

        return store.Value;
    }

    /// <summary>
    /// Internal storage class that tracks whether an original font size has been recorded and its value.
    /// </summary>
    private sealed class FontSizeStore
    {
        /// <summary>Whether an original font size has been stored for this control.</summary>
        public bool HasValue { get; set; }

        /// <summary>The original font size before scaling.</summary>
        public double Value { get; set; }
    }
}
