using wt_lab6_6minimalapi_baranova.Data;
using wt_lab6_6minimalapi_baranova.Models;

var builder = WebApplication.CreateBuilder(args);

// ---- OpenAPI (Swagger) ----
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
    o.SwaggerDoc("v1", new()
    {
        Title = "Products API",
        Version = "v1",
        Description = "REST API интернет-магазина на ASP.NET Core Minimal API"
    }));

var app = builder.Build();

// Документ OpenAPI: /swagger/v1/swagger.json, интерфейс: /swagger
app.UseSwagger();
app.UseSwaggerUI();
//commit
//commit

// Проверка данных из тела запроса -> 400 Bad Request
static List<string> Validate(Product p)
{
    var errors = new List<string>();
    if (string.IsNullOrWhiteSpace(p.Name)) errors.Add("Name: введите название товара");
    if (string.IsNullOrWhiteSpace(p.Category)) errors.Add("Category: введите категорию");
    if (p.Price <= 0) errors.Add("Price: цена должна быть больше 0");
    return errors;
}

// ---- Информация ----
app.MapGet("/", () => "Minimal API работает!");

app.MapGet("/api/info", () => new
{
    Application = "Web Technologies",
    Technology = "ASP.NET Core Minimal API",
    Version = "1.0"
});

// ---- GET ----
app.MapGet("/api/products", (string? search) =>
{
    var products = ProductStore.Products.AsEnumerable();

    if (!string.IsNullOrWhiteSpace(search))
    {
        products = products.Where(x =>
            x.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
    }

    return Results.Ok(products);
})
.WithTags("Products").Produces<List<Product>>(StatusCodes.Status200OK);

app.MapGet("/api/products/{id}", (int id) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);

    if (product == null)
        return Results.NotFound();

    return Results.Ok(product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status200OK).Produces(StatusCodes.Status404NotFound);

// ---- POST ----
app.MapPost("/api/products", (Product product) =>
{
    var errors = Validate(product);
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    product.Id = ProductStore.NextId();   // Id создаёт сервер
    ProductStore.Products.Add(product);

    return Results.Created($"/api/products/{product.Id}", product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status201Created).Produces(StatusCodes.Status400BadRequest);

// ---- PUT ----
app.MapPut("/api/products/{id}", (int id, Product updatedProduct) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);
    if (product == null)
        return Results.NotFound();

    var errors = Validate(updatedProduct);
    if (errors.Count > 0)
        return Results.BadRequest(new { errors });

    product.Name = updatedProduct.Name;
    product.Category = updatedProduct.Category;
    product.Price = updatedProduct.Price;
    product.IsAvailable = updatedProduct.IsAvailable;

    return Results.Ok(product);
})
.WithTags("Products").Produces<Product>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status400BadRequest).Produces(StatusCodes.Status404NotFound);

// ---- DELETE ----
app.MapDelete("/api/products/{id}", (int id) =>
{
    var product = ProductStore.Products.FirstOrDefault(x => x.Id == id);
    if (product == null)
        return Results.NotFound();

    ProductStore.Products.Remove(product);
    return Results.NoContent();
})
.WithTags("Products").Produces(StatusCodes.Status204NoContent).Produces(StatusCodes.Status404NotFound);

// ---- Запуск: порт из переменной окружения PORT (Render) ----
var port = Environment.GetEnvironmentVariable("PORT") ?? "10000";
app.Run($"http://0.0.0.0:{port}");
