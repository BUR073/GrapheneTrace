using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    public class PatientClinician
    {
        public int PatientId { get; set; }
        public int ClinicianId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public ApplicationUser Patient { get; set; } = null!;

        [ForeignKey(nameof(ClinicianId))]
        public ApplicationUser Clinician { get; set; } = null!;
    }
}