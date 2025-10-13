using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrapheneTrace.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        // To display all available roles
        public List<string> Roles { get; set; } = new();

        // To hold the roles selected by the admin
        [Display(Name = "Roles")]
        public List<string> SelectedRoles { get; set; } = new();
    }
}