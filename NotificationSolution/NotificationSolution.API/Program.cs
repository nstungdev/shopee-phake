using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NotificationSolution.API.Data;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NotificationDbContext>(x =>
{
    x.UseNpgsql(builder.Configuration["Database:ConnectionString"]!, opt =>
    {
        opt.EnableRetryOnFailure(configuration.GetValue<int?>("Database:RetryConnectionOnFailure") ?? 5);
        opt.CommandTimeout(configuration.GetValue<int?>("Database:CommandTimeout") ?? 30);
    });
}, ServiceLifetime.Scoped);

#region MassTransit
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
    x.SetKebabCaseEndpointNameFormatter();

    var assembly = typeof(Program).Assembly;
    x.AddConsumers(assembly);
    x.AddActivities(assembly);
    
    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.ConfigureEndpoints(ctx);
    });
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();
