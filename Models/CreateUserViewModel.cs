using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrapheneTrace.Models
{
    public class CreateUserViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Role")]
        public string SelectedRole { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> Roles { get; set; } = new List<SelectListItem>();
    }
}