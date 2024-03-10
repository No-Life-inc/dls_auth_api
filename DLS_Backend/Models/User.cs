
namespace DLS_Backend.Models;

public partial class User
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public string Name { get; set; } = null!;

    public string Password { get; set; } = null!;
    
    public DateTime? CreatedAt { get; set; }

    public string Email { get; set; } = null!;
}
