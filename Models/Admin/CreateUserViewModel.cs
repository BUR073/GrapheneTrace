// SID: 2408078
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models.Admin
{
    /// <summary>
    /// Model for creating a new user
    /// </summary>
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
            
        [Required]
        public String Name { get; set; } = string.Empty;
            
        [Required]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Role")]
        public string SelectedRole { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}

