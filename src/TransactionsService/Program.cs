using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using FluxoCaixa.Transactions.Data;
using FluxoCaixa.Transactions.Services;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DB
builder.Services.AddDbContext<TransactionsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TransactionsConnection") ?? "Data Source=transactions.db"));

// Polly retry policy for health check http client
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => (int)msg.StatusCode == 429)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromMilliseconds(200 * retryAttempt));
}

builder.Services.AddHttpClient<IConsolidatedHealthService, ConsolidatedHealthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Consolidated:BaseUrl"] ?? "http://localhost:5001");
}).AddPolicyHandler(GetRetryPolicy());

// Event publisher (RabbitMQ) - best-effort
builder.Services.AddSingleton<IEventPublisher, RabbitMqEventPublisher>();
builder.Services.AddScoped<ITransactionService, TransactionService>();

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
    var db = scope.ServiceProvider.GetRequiredService<TransactionsDbContext>();
    db.Database.EnsureCreated();
}

app.Run();
