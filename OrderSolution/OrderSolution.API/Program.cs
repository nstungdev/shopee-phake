using Microsoft.EntityFrameworkCore;
using OrderSolution.API.Data;
using OrderSolution.API.Services;
using OrderSolution.API.Settings;
using System.Text.Json.Serialization;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
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

builder.Services.AddOptions<RabbitMqTransportOptions>().Configure(options => {
    var portNr = configuration.GetValue<ushort>("RabbitMQ:Port");
    options.Host = configuration["RabbitMQ:Host"];
    options.Port = portNr;
    options.User = configuration["RabbitMQ:Username"];
    options.Pass = configuration["RabbitMQ:Password"];
    options.VHost = "/";
});

builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<OrderDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(30);
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(1);
        o.UsePostgres();
        o.UseBusOutbox();
        o.DisableInboxCleanupService();
    });

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ConfigureEndpoints(ctx);
    });
});

#region Services
builder.Services.AddScoped<IOrderService, OrderService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
