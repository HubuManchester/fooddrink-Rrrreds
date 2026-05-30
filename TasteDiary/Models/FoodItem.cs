using System.Text.Json.Serialization;

namespace TasteDiary.Models;

/// <summary>
/// Represents a food or drink item with nutritional information, allergen notes, and an associated image.
/// Used throughout the app for display, search, and data persistence via mockapi.io or local fallback.
/// </summary>
public sealed class FoodItem
{
    /// <summary>Unique identifier, auto-generated as a GUID.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    /// <summary>Display name of the food or drink.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>Category: Breakfast, Lunch, Dinner, Snack, or Drink.</summary>
    [JsonPropertyName("category")]
    public string Category { get; set; } = string.Empty;

    /// <summary>A short description including ingredients, flavour profile, and cultural context.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Total energy in kilocalories (kcal).</summary>
    [JsonPropertyName("calories")]
    public int Calories { get; set; }

    /// <summary>Protein content in grams.</summary>
    [JsonPropertyName("protein")]
    public int Protein { get; set; }

    /// <summary>Carbohydrate content in grams.</summary>
    [JsonPropertyName("carbs")]
    public int Carbs { get; set; }

    /// <summary>Fat content in grams.</summary>
    [JsonPropertyName("fat")]
    public int Fat { get; set; }

    /// <summary>Allergen information. Defaults to a fallback message if not explicitly set.</summary>
    [JsonPropertyName("allergyNote")]
    public string AllergyNote { get; set; } = string.Empty;

    /// <summary>Searchable tags used for filtering in the search bar.</summary>
    [JsonPropertyName("tags")]
    public string Tags { get; set; } = string.Empty;

    /// <summary>File name of the embedded image within the app package (e.g. "FoodImages/beef_noodle_soup.jpeg").</summary>
    [JsonIgnore]
    public string ImageName { get; set; } = string.Empty;

    /// <summary>Loaded image source bound to UI controls. Lazy-loaded by FoodCatalogService.</summary>
    [JsonIgnore]
    public ImageSource? ItemImage { get; set; }

    /// <summary>Formatted calorie string for display (e.g. "580 kcal").</summary>
    [JsonIgnore]
    public string CaloriesLabel => $"{Calories} kcal";

    /// <summary>Formatted macronutrient summary for display (e.g. "Protein 32g, carbs 68g, fat 18g").</summary>
    [JsonIgnore]
    public string MacroSummary => $"Protein {Protein}g, carbs {Carbs}g, fat {Fat}g";

    /// <summary>A complete accessible summary used by the Text-to-Speech engine to describe the item aloud.</summary>
    [JsonIgnore]
    public string AccessibleSummary => $"{Name}. {Category}. {Calories} kcal. {MacroSummary}. {AllergyNote}";
}
