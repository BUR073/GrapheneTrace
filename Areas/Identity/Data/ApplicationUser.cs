using Microsoft.AspNetCore.Identity;

namespace GrapheneTrace.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}