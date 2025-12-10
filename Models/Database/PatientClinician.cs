// SID: 2408078
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;


namespace GrapheneTrace.Models.Database
{   
    public class PatientClinician
    {
        public int PatientId { get; set; }
        
        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; } = null!;
        
        public int ClinicianId { get; set; }
        
        [ForeignKey(nameof(ClinicianId))]
        public ApplicationUser Clinician { get; set; } = null!;
    }
}