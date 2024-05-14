namespace DLS_Backend.Controller;

public class Hashing
{

    /// <summary>
    /// Hashes the password using BCrypt.
    /// </summary>
    /// <param name="password">The pre-hashed password as a string. This is the password in its plain text form before hashing.</param>
    /// <returns>The hashed password as a string. The hash is generated using the BCrypt algorithm, suitable for securely storing passwords.</returns>
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password); 
    }
    
    /// <summary>
    /// Verifies the password using BCrypt
    /// </summary>
    /// <param name="password">The pre-hashed password as a string. This is the password in its plain text form before hashing.</param>
    /// <param name="hash">The hashed password as a string. The hash is from the user on the database.</param>
    /// <returns></returns>
    public bool Verify(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
    
}