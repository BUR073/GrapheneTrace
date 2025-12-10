// SID: 2408078
namespace GrapheneTrace.Models.Admin
{
    /// <summary>
    /// Model for storing the details of a user to be passed to Admin home
    /// </summary>
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<string?> Roles { get; set; } = [];

        public String Name { get; set; } = string.Empty;
        
        public DateTime DateOfBirth { get; set; }
        
        public int PatientLinkCount { get; set; }
        public int ClinicianLinkCount { get; set; }
    }
}

