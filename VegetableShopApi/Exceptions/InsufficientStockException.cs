namespace VegetableShopApi.Exceptions;

public class InsufficientStockException : Exception
{
    public InsufficientStockException
    (int productId,
        decimal requestedQuantity,
        decimal availableQuantity)
        : base($"Insufficient stock for product {productId}. " +
               $"Requested: {requestedQuantity}, Available: {availableQuantity}.")
    {
    }
    
}