using DefaultNamespace;
using Microsoft.EntityFrameworkCore;
using Serilog;
using VegetableShopApi.Data;
using VegetableShopApi.Enums;
using VegetableShopApi.Services;
using VegetableShopApi.Services.Interfaces;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Error)
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .WriteTo.File(
        $"logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Host.UseSerilog();
builder.Services.AddDbContext<AppDbContext>( options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.MapEnum<UnitType>("unit");
            o.MapEnum<OrderStatus>("order_status");
            o.MapEnum<UserRole>("role");
        }));
builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.UseCors("Angular");

app.UseHttpsRedirection();



app.Run();

