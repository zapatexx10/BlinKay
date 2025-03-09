using BlinkayTest.Application;
using BlinKayTest.API.Helpers;
using BlinKayTest.Contracts;
using BlinKayTest.Infrastructure.SQL;
using BlinKayTest.Infrastructure.SQL.Context;
using BlinKayTest.Infrastructure.SQL.UnitOfWork;
using BlinKayTest.Shared;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IBenchmarkRepository, BenchmarkRepository>();
builder.Services.AddScoped<IBenchmarkService, BenchmarkService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await MigrationsHelper.MigrateDatabaseAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
