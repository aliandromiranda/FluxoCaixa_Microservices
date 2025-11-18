using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Consolidated.Data;
using FluxoCaixa.Consolidated.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IConsolidationService, ConsolidationService>();
builder.Services.AddSingleton<IEventConsumer, RabbitMqEventConsumer>();
builder.Services.AddHostedService<RabbitBackgroundService>();

builder.Services.AddDbContext<ConsolidatedDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("ConsolidatedConnection") ?? "Data Source=consolidated.db"));

builder.Services.AddSingleton<IRedisCache, RedisCacheFallback>();
builder.Services.AddSingleton<IEventConsumer, RabbitMqEventConsumer>();
builder.Services.AddScoped<IConsolidationService, ConsolidationService>();
builder.Services.AddHostedService<RabbitBackgroundService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ConsolidatedDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
