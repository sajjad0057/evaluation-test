
using System.ComponentModel.DataAnnotations;

namespace ManageUser.Infrastructure.Entites;

public class User
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}
