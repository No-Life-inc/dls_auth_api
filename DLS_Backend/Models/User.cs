
namespace DLS_Backend.Models;

public partial class User
{
    public int id { get; set; }

    public string guid { get; set; }

    public string first_name { get; set; } = null!;
    
    public string last_name { get; set; } = null!;

    public string password { get; set; } = null!;
    
    public DateTime? created_at { get; set; }

    public string email { get; set; } = null!;
}
