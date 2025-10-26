using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;


namespace GrapheneTrace.Models.Database
{
    public class PatientClinician
    {
        [Key, Column(Order = 0)]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; } = null!;

        [Key, Column(Order = 1)]
        public int ClinicianId { get; set; }

        [ForeignKey(nameof(ClinicianId))]
        public ApplicationUser Clinician { get; set; } = null!;
    }
}