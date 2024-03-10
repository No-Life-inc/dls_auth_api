namespace DLS_Backend.Controller;

public class Hashing
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password); 
    }
    
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    
}