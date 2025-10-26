using System.Collections.Generic;

namespace GrapheneTrace.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();

        public String Name { get; set; } = string.Empty;
        
        public DateTime DateOfBirth { get; set; }
    }
}