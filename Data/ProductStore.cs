using wt_lab6_6minimalapi_baranova.Models;

namespace wt_lab6_6minimalapi_baranova.Data;

// Единая коллекция в памяти для всех API-операций
public static class ProductStore
{
    public static List<Product> Products { get; } = new()
    {
        new Product { Id = 1, Name = "Ноутбук",    Category = "Компьютеры", Price = 2500, IsAvailable = true  },
        new Product { Id = 2, Name = "Мышь",       Category = "Аксессуары", Price = 80,   IsAvailable = true  },
        new Product { Id = 3, Name = "Клавиатура", Category = "Аксессуары", Price = 150,  IsAvailable = false },
        new Product { Id = 4, Name = "Монитор",    Category = "Компьютеры", Price = 900,  IsAvailable = true  },
        new Product { Id = 5, Name = "Наушники",   Category = "Аудио",      Price = 320,  IsAvailable = true  },
        new Product { Id = 6, Name = "Смартфон",   Category = "Телефоны",   Price = 1800, IsAvailable = true  }
    };

    public static int NextId() => Products.Count == 0 ? 1 : Products.Max(x => x.Id) + 1;
}
