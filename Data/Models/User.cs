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

        public User(int Id)
        {
            this.Id = Id;
        }

        public User(string Username, string Password)
        {
            this.Username = Username;
            this.Password = Password;
        }
    }
}
