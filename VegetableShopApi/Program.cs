using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Data;
using VegetableShopApi.Enums;
using VegetableShopApi.Services;
using VegetableShopApi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<AppDbContext>( options => 
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
        o =>
        {
            o.MapEnum<UnitType>("unit");
            o.MapEnum<OrderStatus>("order_status");
            o.MapEnum<UserRole>("role");
        }));
builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.UseHttpsRedirection();



app.Run();

