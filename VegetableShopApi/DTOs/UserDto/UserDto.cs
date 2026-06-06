using VegetableShopApi.Models;

namespace VegetableShopApi.DTOs.UserDto;

public class UserDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }

    public UserDto(User user)
    {
        Id = user.Id;
        Name = user.FirstName + " " + user.LastName;
        Email = user.Email;
        Phone = user.Phone;
    }
}