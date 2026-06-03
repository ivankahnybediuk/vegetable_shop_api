using Microsoft.EntityFrameworkCore;
using VegetableShopApi.Data;
using VegetableShopApi.Enums;

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


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();



app.Run();

