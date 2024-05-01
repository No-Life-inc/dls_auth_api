namespace DLS_Backend.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        public int id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime? created_at { get; set; }
    }
}
