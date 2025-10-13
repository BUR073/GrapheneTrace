using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GrapheneTrace.Models
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public List<string> Roles { get; set; } = new();
        
        [Display(Name = "Roles")]
        public List<string> SelectedRoles { get; set; } = new();
    }
}