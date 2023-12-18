// NOTE: dotnet run --urls="https://localhost:7443" to specify https and port 7443
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Pizzas") ?? "Data Source=Pizzas.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "PizzaStore API",
            Description = "Making the Pizzas you live",
            Version = "v1"
        });
});

// uncomment ONE of the following builder.Services.AddDbContext calls:
//  uncomment to use in memory db
//builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
//  uncomment to use sqlite
builder.Services.AddDbContext<PizzaDb>(options => options.UseSqlite(connectionString));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
});

app.UseHttpsRedirection();


app.MapGet("/", () => "Hello Web");

var pizzas = app.MapGroup("/pizzas");
pizzas.MapGet("/{id}", GetPizza);
pizzas.MapGet("/", GetPizzas);
pizzas.MapPost("/", CreatePizza);
pizzas.MapPut("/{id}", UpdatePizza);
pizzas.MapDelete("/{id}", RemovePizza);


app.Run();


static async Task<IResult> GetPizza(int id, PizzaDb db)
{
    var pizza = await db.GetPizza(id);
    return pizza == null ? TypedResults.NotFound() : TypedResults.Ok(pizza);
}

static async Task<IResult> GetPizzas(PizzaDb db)
{
    return TypedResults.Ok(await db.GetPizzas());
}

static async Task<IResult> CreatePizza(Pizza pizza, PizzaDb db)
{
    var newPizza = await db.CreatePizza(pizza);
    return TypedResults.Created($"/pizzas/{newPizza.Id}", newPizza);
}

static async Task<IResult> UpdatePizza(int id, Pizza pizza, PizzaDb db)
{
    await db.UpdatePizza(id, pizza);
    return TypedResults.NoContent();
}

static async Task<IResult> RemovePizza(int id, PizzaDb db)
{
    var deleted = await db.RemovePizza(id);
    return deleted == null ? TypedResults.NotFound() : TypedResults.NoContent();
}