using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GrapheneTrace.Areas.Identity.Data;

namespace GrapheneTrace.Models
{
    [Table("Data", Schema = "GrapheneTrace")]
    public class Data 
    {
        [Key]
        public int DataId { get; set; }
        public int UserId { get; set; } 
        [ForeignKey(nameof(UserId))] 
        public ApplicationUser User { get; set; } = null!;
        public DateTime Timestamp { get; set; }

    }

}