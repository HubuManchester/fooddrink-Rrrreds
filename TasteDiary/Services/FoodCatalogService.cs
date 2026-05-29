using System.Net.Http.Json;
using System.Text.Json;
using TasteDiary.Models;

namespace TasteDiary.Services;

public static class FoodCatalogService
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly List<FoodItem> LocalFallbackItems =
    [
        new()
        {
            Name = "Braised Beef Noodle Soup",
            Category = "Lunch",
            Description = "Hand-pulled noodles with braised beef shank, bok choy, and rich bone broth, topped with scallions and coriander.",
            Calories = 580,
            Protein = 32,
            Carbs = 68,
            Fat = 18,
            AllergyNote = "Contains gluten and beef.",
            Tags = "chinese lunch beef noodles popular",
            ImageName = "FoodImages/beef_noodle_soup.jpeg"
        },
        new()
        {
            Name = "Bubble Milk Tea",
            Category = "Drink",
            Description = "Classic Taiwanese milk tea with chewy tapioca pearls. Half-sugar or less-sugar options recommended.",
            Calories = 320,
            Protein = 4,
            Carbs = 56,
            Fat = 8,
            AllergyNote = "Contains dairy. Pearls contain tapioca starch.",
            Tags = "drink milk tea dessert taiwan",
            ImageName = "FoodImages/bubble_tea.jpeg"
        },
        new()
        {
            Name = "Tomato Egg Rice Bowl",
            Category = "Lunch",
            Description = "A classic home-style dish — soft scrambled eggs with sweet-and-sour tomatoes served over steamed white rice.",
            Calories = 450,
            Protein = 18,
            Carbs = 62,
            Fat = 14,
            AllergyNote = "Contains eggs.",
            Tags = "chinese home-style lunch rice quick",
            ImageName = "FoodImages/tomato_egg_rice.jpeg"
        },
        new()
        {
            Name = "Jianbing Pancake",
            Category = "Breakfast",
            Description = "Mung bean crepe with egg, sweet bean paste and chilli sauce, wrapped around a crispy cracker or youtiao.",
            Calories = 380,
            Protein = 14,
            Carbs = 44,
            Fat = 16,
            AllergyNote = "Contains gluten, eggs, and sesame.",
            Tags = "breakfast chinese street-food popular",
            ImageName = "FoodImages/jianbing_pancake.jpeg"
        },
        new()
        {
            Name = "Mala Stir-Fry Pot",
            Category = "Dinner",
            Description = "Choose-your-own meats, seafood, and vegetables wok-fried with Sichuan peppercorns and dried chillies — numbing, spicy, and aromatic.",
            Calories = 680,
            Protein = 42,
            Carbs = 28,
            Fat = 38,
            AllergyNote = "Contains soy and chilli. May contain shellfish.",
            Tags = "chinese dinner mala spicy sharing",
            ImageName = "FoodImages/mala_stir_fry.jpg"
        },
        new()
        {
            Name = "Xiaolongbao (Soup Dumplings)",
            Category = "Lunch",
            Description = "Thin-skinned dumplings filled with minced pork and piping-hot broth. Served with shredded ginger and vinegar. Six per steamer.",
            Calories = 420,
            Protein = 24,
            Carbs = 38,
            Fat = 20,
            AllergyNote = "Contains gluten and pork.",
            Tags = "chinese lunch dim-sum steamed shanghai",
            ImageName = "FoodImages/xiaolongbao.jpg"
        },
        new()
        {
            Name = "Soy Milk & Youtiao",
            Category = "Breakfast",
            Description = "Freshly ground sweet soy milk paired with crispy deep-fried dough sticks — a classic Chinese breakfast combo.",
            Calories = 350,
            Protein = 12,
            Carbs = 42,
            Fat = 16,
            AllergyNote = "Contains gluten and soy. Youtiao is deep-fried.",
            Tags = "breakfast chinese classic soy-milk youtiao",
            ImageName = "FoodImages/soy_milk_youtiao.jpg"
        },
        new()
        {
            Name = "Mango Pomelo Sago",
            Category = "Drink",
            Description = "A classic Hong Kong dessert drink with mango, pomelo, coconut milk, and sago pearls — sweet, creamy, and refreshing.",
            Calories = 280,
            Protein = 3,
            Carbs = 48,
            Fat = 10,
            AllergyNote = "Contains mango and coconut milk.",
            Tags = "drink dessert hong-kong summer mango",
            ImageName = "FoodImages/mango_pomelo_sago.jpeg"
        }
    ];

    private static List<FoodItem> cachedItems = new(LocalFallbackItems);

    public static bool LastLoadUsedMockApi { get; private set; }

    public static async Task<IReadOnlyList<FoodItem>> SearchAsync(string? query)
    {
        var items = await GetAllAsync();

        if (string.IsNullOrWhiteSpace(query))
        {
            return items.OrderBy(item => item.Name).ToList();
        }

        var normalised = query.Trim();
        return items
            .Where(item =>
                item.Name.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Category.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Description.Contains(normalised, StringComparison.OrdinalIgnoreCase) ||
                item.Tags.Contains(normalised, StringComparison.OrdinalIgnoreCase))
            .OrderBy(item => item.Name)
            .ToList();
    }

    public static async Task<FoodItem?> GetByIdAsync(string id)
    {
        if (MockApiConfig.IsConfigured)
        {
            try
            {
                var item = await HttpClient.GetFromJsonAsync<FoodItem>(
                    $"{MockApiConfig.EndpointUrl.TrimEnd('/')}/{Uri.EscapeDataString(id)}",
                    JsonOptions);

                if (item is not null)
                {
                    return item;
                }
            }
            catch
            {
                // Fall back to the last loaded cache below.
            }
        }

        return cachedItems.FirstOrDefault(item => item.Id == id);
    }

    public static async Task<FoodItem> AddAsync(FoodItem item)
    {
        if (MockApiConfig.IsConfigured)
        {
            var response = await HttpClient.PostAsJsonAsync(MockApiConfig.EndpointUrl, item, JsonOptions);
            response.EnsureSuccessStatusCode();

            var created = await response.Content.ReadFromJsonAsync<FoodItem>(JsonOptions);
            if (created is not null)
            {
                cachedItems.Add(created);
                return created;
            }
        }

        cachedItems.Add(item);
        return item;
    }

    private static async Task<IReadOnlyList<FoodItem>> GetAllAsync()
    {
        if (!MockApiConfig.IsConfigured)
        {
            LastLoadUsedMockApi = false;
            return cachedItems;
        }

        try
        {
            var items = await HttpClient.GetFromJsonAsync<List<FoodItem>>(MockApiConfig.EndpointUrl, JsonOptions);
            if (items is { Count: > 0 })
            {
                cachedItems = items;
                LastLoadUsedMockApi = true;
                return cachedItems;
            }
        }
        catch
        {
            // Keep the app usable during demos even if the network is unavailable.
        }

        LastLoadUsedMockApi = false;
        return cachedItems;
    }
}
