namespace DLS_Backend.Models;


public class UserTombstone
{
    public int UserId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime DeletionDate { get; set; }
    public User User { get; set; }
}

public class DeleteUserRequest
{
    public Guid guid { get; set; }
    public string password { get; set; }
    public string token { get; set; }
}
