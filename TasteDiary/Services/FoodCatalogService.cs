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
            Name = "红烧牛肉面",
            Category = "午餐",
            Description = "手工拉面配红烧牛腱、青菜和浓郁牛骨汤，撒上葱花和香菜。",
            Calories = 580,
            Protein = 32,
            Carbs = 68,
            Fat = 18,
            AllergyNote = "含麸质和牛肉。",
            Tags = "中式 午餐 牛肉 面食 热门"
        },
        new()
        {
            Name = "珍珠奶茶",
            Category = "饮品",
            Description = "经典台式奶茶配Q弹珍珠，可选择半糖或少糖。",
            Calories = 320,
            Protein = 4,
            Carbs = 56,
            Fat = 8,
            AllergyNote = "含乳制品。珍珠含木薯淀粉。",
            Tags = "饮品 奶茶 甜品 台湾"
        },
        new()
        {
            Name = "番茄炒蛋盖饭",
            Category = "午餐",
            Description = "经典家常菜——嫩滑炒蛋配酸甜番茄，盖在热腾腾的白米饭上。",
            Calories = 450,
            Protein = 18,
            Carbs = 62,
            Fat = 14,
            AllergyNote = "含鸡蛋。",
            Tags = "中式 家常 午餐 米饭 快手"
        },
        new()
        {
            Name = "煎饼果子",
            Category = "早餐",
            Description = "绿豆面薄饼摊鸡蛋，刷甜面酱和辣酱，夹油条或薄脆。",
            Calories = 380,
            Protein = 14,
            Carbs = 44,
            Fat = 16,
            AllergyNote = "含麸质、鸡蛋和芝麻。",
            Tags = "早餐 中式 街头小吃 热门"
        },
        new()
        {
            Name = "麻辣香锅",
            Category = "晚餐",
            Description = "自选肉类、海鲜和蔬菜，以花椒和干辣椒爆炒，麻辣鲜香。",
            Calories = 680,
            Protein = 42,
            Carbs = 28,
            Fat = 38,
            AllergyNote = "含大豆、辣椒。可能含贝类。",
            Tags = "中式 晚餐 麻辣 聚餐 重口味"
        },
        new()
        {
            Name = "小笼包",
            Category = "午餐",
            Description = "薄皮包裹鲜猪肉馅和滚烫汤汁，配姜丝和醋蘸食。一笼六只。",
            Calories = 420,
            Protein = 24,
            Carbs = 38,
            Fat = 20,
            AllergyNote = "含麸质和猪肉。",
            Tags = "中式 午餐 点心 蒸品 上海"
        },
        new()
        {
            Name = "豆浆油条",
            Category = "早餐",
            Description = "现磨甜豆浆配现炸酥脆油条，经典中式早餐搭配。",
            Calories = 350,
            Protein = 12,
            Carbs = 42,
            Fat = 16,
            AllergyNote = "含麸质和大豆。油条为油炸食品。",
            Tags = "早餐 中式 经典 豆浆 油条"
        },
        new()
        {
            Name = "杨枝甘露",
            Category = "饮品",
            Description = "芒果、西柚、椰奶和西米的经典港式甜品饮品，清甜解暑。",
            Calories = 280,
            Protein = 3,
            Carbs = 48,
            Fat = 10,
            AllergyNote = "含芒果和椰奶。",
            Tags = "饮品 甜品 港式 夏日 芒果"
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
