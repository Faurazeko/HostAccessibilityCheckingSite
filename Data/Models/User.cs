using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HostAccessibilityCheckingSite.Data.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public User(int id)
        {
            Id = id;
        }

        public User(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}
