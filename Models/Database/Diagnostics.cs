using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrapheneTrace.Models.Database
{
    public class Diagnostics
    {
        [Key] public int DiagnosticsID { get; set; }

        public int DataId { get; set; }
        [ForeignKey(nameof(DataId))] 
        public required Data Data{ get; set; }

        public string PatientCondition { get; set; } = string.Empty;

        public string Medication { get; set; } = string.Empty;

    }
}
