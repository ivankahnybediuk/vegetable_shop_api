using VegetableShopApi.Enums;

namespace VegetableShopApi.Models;

public class OrderItem
{
   public int Id { get; set; }
   public int OrderId { get; set; }
   public required Order Order { get; set; }
   public int ProductId { get; set; }
   public Product Product { get; set; }
   public decimal Quantity {get; set;}
   public decimal UnitPrice {get; set;}
   public decimal TotalPrice {get; set;}
   public UnitType Unit {get; set;}
}