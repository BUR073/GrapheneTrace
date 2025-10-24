namespace GrapheneTrace.Areas.Identity.Data; 
using Microsoft.AspNetCore.Identity;
public class ApplicationUser : IdentityUser<int>
{
    public string? Name { get; set; }
    public DateTime? DateOfBirth { get; set; }
}