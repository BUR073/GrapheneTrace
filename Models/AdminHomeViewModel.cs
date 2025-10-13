using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace GrapheneTrace.Models
{
    public class AdminHomeViewModel
    {
        public List<UserViewModel> Users { get; set; } = new List<UserViewModel>();
    }
}