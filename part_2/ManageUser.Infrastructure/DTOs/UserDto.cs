
using System.ComponentModel.DataAnnotations;

namespace ManageUser.Infrastructure.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Age is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Age should not be less than 1 year")]
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}