using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool IsAdmin { get; set; }

        public User(int id)
        {
            Id = id;
        }

        public User(string username, string password, bool isAdmin = false)
        {
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
        }
    }
}
