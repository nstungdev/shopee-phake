using Microsoft.EntityFrameworkCore;
using OrderSolution.API.Data;
using OrderSolution.API.Settings;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>(configuration.GetSection("Database"));

builder.Services.AddDbContext<OrderDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]!, opt =>
    {
        opt.EnableRetryOnFailure(configuration.GetValue<int?>("Database:RetryConnectionOnFailure") ?? 5);
        opt.CommandTimeout(configuration.GetValue<int?>("Database:CommandTimeout") ?? 30);
    });
}, ServiceLifetime.Scoped);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
