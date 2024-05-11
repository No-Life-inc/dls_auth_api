using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace DLS_Backend.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class UserInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; }
        
        [Required]
        public DateTime? created_at { get; set; }
        
        // Foreign key for Users table
        public int UserId { get; set; }

        // Navigation property for Users table
        [ForeignKey("id")]
        public User User { get; set; }
    }
    
    
    /// <summary>
    /// Register request model
    /// </summary>
    /// <param name="first_name">Users first name</param>
    /// <param name="last_name">Users last name</param>
    /// <param name="email">Users email</param>
    /// <param name="password">Users hashed password</param>
    public class RegisterRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public Guid guid { get; set; }
    }

    /// <summary>
    /// Login request model
    /// </summary>
    /// <param name="Email">Users email</param>
    /// <param name="Password">Users hashed password</param>
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class EditUserRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string token { get; set; }
    }
    
}