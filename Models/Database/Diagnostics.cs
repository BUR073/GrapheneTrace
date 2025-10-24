using GrapheneTrace.Models.Database;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrapheneTrace.Models.Database
{
    public class Diagnostics
    {
        [Key] public int DiagnosticsID { get; set; }

        public int DatID { get; set; }
        [ForeignKey(nameof(DataID))] 
        public Data DataID { get; set; }

        public string PatientCondition { get; set; } = string.empty()

        public string Medication { get; set; } = string.empty()

    }
}
