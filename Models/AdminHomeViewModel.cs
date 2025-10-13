using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GrapheneTrace.Models
{
    public class AdminHomeViewModel
    {
        public List<IdentityUser<int>> Users { get; set; } = new List<IdentityUser<int>>();
    }
}