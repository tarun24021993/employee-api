// var builder = WebApplication.CreateBuilder(args);

// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();

// var app = builder.Build();

// // Configure the HTTP request pipeline.
// //if (app.Environment.IsDevelopment())
// //{
//     app.UseSwagger();
//     app.UseSwaggerUI();
//     //tarun
// //}

// //app.UseHttpsRedirection();

// var summaries = new[]
// {
//     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// };



// app.MapGet("/weatherforecast", () =>
// {
//     var forecast =  Enumerable.Range(1, 5).Select(index =>
//         new WeatherForecast
//         (
//             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//             Random.Shared.Next(-20, 55),
//             summaries[Random.Shared.Next(summaries.Length)]
//         ))
//         .ToArray();
//     return forecast;
// })
// .WithName("GetWeatherForecast")
// .WithOpenApi();

// app.MapGet("/environment", (IConfiguration config) =>
// {
//     return Results.Ok(new
//     {
//         Environment = config["EnvironmentName"]
//     });
// });

// app.Run();

// record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// {
//     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// }


using EmployeeApi.Data;
using EmployeeApi.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<EmployeeDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Employee API is running");

app.MapGet("/employees", async (EmployeeDbContext db) =>
{
    return await db.Employees.ToListAsync();
});

app.MapGet("/employees/{id}", async (int id, EmployeeDbContext db) =>
{
    var employee = await db.Employees.FindAsync(id);

    return employee is null
        ? Results.NotFound()
        : Results.Ok(employee);
});

app.MapPost("/employees", async (Employee employee, EmployeeDbContext db) =>
{
    db.Employees.Add(employee);
    await db.SaveChangesAsync();

    return Results.Created($"/employees/{employee.Id}", employee);
});

app.MapPut("/employees/{id}", async (int id, Employee updatedEmployee, EmployeeDbContext db) =>
{
    var employee = await db.Employees.FindAsync(id);

    if (employee is null)
        return Results.NotFound();

    employee.Name = updatedEmployee.Name;
    employee.Department = updatedEmployee.Department;

    await db.SaveChangesAsync();

    return Results.Ok(employee);
});

app.MapDelete("/employees/{id}", async (int id, EmployeeDbContext db) =>
{
    var employee = await db.Employees.FindAsync(id);

    if (employee is null)
        return Results.NotFound();

    db.Employees.Remove(employee);

    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();