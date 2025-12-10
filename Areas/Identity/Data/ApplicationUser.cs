// SID: 2408078
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Models.Database;

namespace GrapheneTrace.Areas.Identity.Data
{
    public class ApplicationUser : IdentityUser<int>
    {
        public required string Name { get; set; }
        public required DateTime DateOfBirth { get; set; }
        public ICollection<Feedback> Feedback { get; set; } = new List<Feedback>();
        public ICollection<FeedbackReply> FeedbackReplies { get; set; } = new List<FeedbackReply>();
        public ICollection<PatientClinician> ClinicianLinks { get; set; } = new List<PatientClinician>(); // Where user is patient
        public ICollection<PatientClinician> PatientLinks { get; set; } = new List<PatientClinician>(); // Where User is clinician
    }
}


