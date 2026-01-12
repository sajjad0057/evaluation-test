
using System.ComponentModel.DataAnnotations;

namespace ManageUser.Infrastructure.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}