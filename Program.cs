using PizzaStore.DB;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
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
app.MapGet("/pizzas/{id}", (int id) => PizzaDB.GetPizza(id));
app.MapGet("/pizzas", () => PizzaDB.GetPizzas());
app.MapPost("/pizzas", (Pizza pizza) => PizzaDB.CreatePizza(pizza));
app.MapPut("/pizzas", (Pizza pizza) => PizzaDB.UpdatePizza(pizza));
app.MapPut("/pizzas/{id}", (int id) => PizzaDB.RemovePizza(id));


app.Run();
