using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using GrapheneTrace.Enums;

namespace GrapheneTrace.Models.Admin
{
    public class ManageLinksViewModel
    {
        public int PrimaryUserId { get; set; }
        public string PrimaryUserName { get; set; } = string.Empty;
        public UserType PrimaryUserRole { get; set; } 
        public List<SelectListItem> AssignedLinks { get; set; } = new();
        
        public List<SelectListItem> AvailableLinks { get; set; } = new();

        public List<int> SelectedLinkIds { get; set; } = new();
    }
}

