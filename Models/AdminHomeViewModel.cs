using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GrapheneTrace.Models
{
    public class AdminHomeViewModel
    {
        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();
    }
}