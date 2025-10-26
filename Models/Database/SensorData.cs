using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models.Database
{
    [Table("SensorData", Schema = "GrapheneTrace")]
    public class SensorData 
    {
        [Key]
        public int DataId { get; set; }
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } = null!;
        public DateTime Timestamp { get; set; }
        public Heatmap? Heatmap { get; set; } 
        public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
        
        public Diagnostics? Diagnostics { get; set; } 
        
        public ICollection<Feedback> Feedback { get; set; } = new List<Feedback>();

    }

}