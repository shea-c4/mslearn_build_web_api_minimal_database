using Microsoft.EntityFrameworkCore;

public record Pizza
{
    public int Id { get; set; }
    public string? Name { get; set; }
}

public class PizzaDb : DbContext
{
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public string DbPath { get; }

    private static List<Pizza> _pizzas = new List<Pizza>()
   {
     new Pizza{ Id=1, Name="Montemagno, Pizza shaped like a great mountain" },
     new Pizza{ Id=2, Name="The Galloway, Pizza shaped like a submarine, silent but deadly"},
     new Pizza{ Id=3, Name="The Noring, Pizza shaped like a Viking helmet, where's the mead"}
   };

    public PizzaDb(DbContextOptions<PizzaDb> options)
     : base(options)
    {
        DbPath = "Pizzas.db";
        this.Database.EnsureCreated();

        if (Pizzas.Count() == 0)
        {
            _pizzas.ForEach(pizza => Pizzas.Add(pizza));
            SaveChanges();
        }
   }

    public async Task<List<Pizza>> GetPizzas()
    {
        return await Pizzas.ToListAsync();
    }

    public async Task<Pizza?> GetPizza(int id)
    {
        return await Pizzas.FindAsync(id);
    }

    public async Task<Pizza> CreatePizza(Pizza pizza)
    {
        var entry = await Pizzas.AddAsync(pizza);
        await SaveChangesAsync();

        return entry.Entity;
    }

    public async Task<Pizza?> UpdatePizza(int id, Pizza update)
    {
        var pizza = await Pizzas.FindAsync(id);
        if (pizza is null) return null;

        // we dont allow changing id
        pizza.Name = update.Name;
        await SaveChangesAsync();

        return update;
    }

    public async Task<Pizza?> RemovePizza(int id)
    {
        var pizza = await Pizzas.FindAsync(id);
        if (pizza == null) return pizza;

        Pizzas.Remove(pizza);
        await SaveChangesAsync();

        return pizza;
    }
}