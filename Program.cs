using Microsoft.EntityFrameworkCore;
using PizzaStore.Models;
using PizzaStore.DB;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Pizza") ?? "Data Source=Pizza.db";

builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddDbContext<PizzaDb>(options => options.UseInMemoryDatabase("items"));
builder.Services.AddSqlite<PizzaDb>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "PizzaStore API", Description = "Making the Pizzas you like", Version = "v1"});
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(C =>
{
    C.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API");
});

app.MapGet("/", () => "Hello World!");
/*Original routes, without EF Core
app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
app.MapPost("/pizzas", (PizzaStore.DB.Pizza pizza) => PizzaDB.CreatePizza(pizza));
app.MapPut("/pizzas", (PizzaStore.DB.Pizza pizza) => PizzaDB.UpdatePizza(pizza));
app.MapPut("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));*/
app.MapGet("/pizza", async (PizzaDb db) => await db.Pizzas.ToListAsync()); //Get pizzas
app.MapPost("/pizza", async (PizzaDb db, PizzaStore.Models.Pizza pizza) => { //Add a pizza
    await db.Pizzas.AddAsync(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizza/{pizza.Id}", pizza);
});
app.MapGet("/pizza/{id}", async (PizzaDb db, int id) => await db.Pizzas.FindAsync(id)); //Get a single pizza by id.
app.MapPut("/pizza/{id}", async (PizzaDb db, PizzaStore.Models.Pizza updatepizza, int id) => { //Update a pizza to change toppings
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) return Results.NotFound();
    pizza.Name = updatepizza.Name;
    pizza.Description = updatepizza.Description;
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/pizza/{id}", async (PizzaDb db, int id) => { //Delete a pizza, such as if someone adds pineapple...
    var pizza = await db.Pizzas.FindAsync(id);
    if (pizza is null) {
        return Results.NotFound();
    }
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    return Results.Ok();
});

app.Run();
