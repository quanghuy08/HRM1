using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<EmployeesDB>(opt => opt.UseInMemoryDatabase("Employees"));
var app = builder.Build();


app.MapGet("/employee", async (EmployeesDB db) =>
    await db.Employeess.ToListAsync());

app.MapGet("/employee/{id}", async (int id, EmployeesDB db) =>
    await db.Employeess.FindAsync(id)
        is Employees employees
            ? Results.Ok(employees)
            : Results.NotFound());

app.MapPost("/employee", async (Employees employees, EmployeesDB db) =>
{
    db.Employeess.Add(employees);
    await db.SaveChangesAsync();

    return Results.Created($"/employee/{employees.Id}", employees);
});

app.MapPut("/employee/{id}", async (int id, Employees inputEmployees, EmployeesDB db) =>
{
    var employees = await db.Employeess.FindAsync(id);

    if (employees is null) return Results.NotFound();

    employees.Name = inputEmployees.Name;

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapDelete("/employee/{id}", async (int id, EmployeesDB db) =>
{
    if (await db.Employeess.FindAsync(id) is Employees employees)
    {
        db.Employeess.Remove(employees);
        await db.SaveChangesAsync();
        return Results.Ok(employees);
    }

    return Results.NotFound();
});

app.Run();

class Employees
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
}

class EmployeesDB : DbContext
{
    public EmployeesDB(DbContextOptions<EmployeesDB> options)
        : base(options) { }

    public DbSet<Employees> Employeess => Set<Employees>();
}