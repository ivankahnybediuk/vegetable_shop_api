using VegetableShopApi.Enums;

namespace VegetableShopApi.Models;

public class User
{
    public int Id { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public UserRole Role { get; set; }
}