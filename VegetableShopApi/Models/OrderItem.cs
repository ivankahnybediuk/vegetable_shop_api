using VegetableShopApi.Enums;

namespace VegetableShopApi.Models;

public class OrderItem
{
   public int Id { get; set; }
   public int OrderId { get; set; }
   public Order? Order { get; set; }
   public int ProductId { get; set; }
   public Product? Product { get; set; }
   public decimal Quantity {get; set;}
   public decimal UnitPrice {get; set;}
   public decimal TotalPrice {get; set;}
   public UnitType Unit {get; set;}

   public static OrderItem CreateOrderItem(Product product, decimal quantity, Order order)
   {
       return new OrderItem()
       {
           Product = product, 
           ProductId = product.Id,
           Quantity = quantity,
           UnitPrice = product.Price,
           TotalPrice = product.Price * quantity,
           Unit = product.Unit
       };
   }
}