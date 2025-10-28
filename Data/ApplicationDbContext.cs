using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using GrapheneTrace.Areas.Identity.Data;
using GrapheneTrace.Models.Database; 

namespace GrapheneTrace.Data
{


    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<ApplicationUser> User { get; set; } 
        public DbSet<Alert> Alert { get; set; }
        public DbSet<SensorData> SensorData { get; set; }
        public DbSet<Diagnostics> Diagnostics { get; set; }
        public DbSet<Feedback> Feedback { get; set; }
        public DbSet<FeedbackReply> FeedbackReply { get; set; }
        public DbSet<Heatmap>  Heatmap { get; set; }
        public DbSet<HeatmapChunk> HeatmapChunk { get; set; }
        public DbSet<PatientClinician> PatientClinician { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); 
            
            builder.Entity<PatientClinician>()
                .HasKey(pc => new { pc.PatientId, pc.ClinicianId });
            
            // Heatmap → HeatmapChunk (1..* | *..1)
            builder.Entity<HeatmapChunk>()
                .HasOne(hc => hc.Heatmap)      
                .WithMany(h => h.Chunks)       
                .HasForeignKey(hc => hc.HeatmapId) 
                .OnDelete(DeleteBehavior.Cascade); 
            
            // SensorData -> Heatmap (1..1 | 1..1)
            builder.Entity<SensorData>()
                .HasOne(sd => sd.Heatmap)              
                .WithOne(h => h.SensorData)            
                .OnDelete(DeleteBehavior.Cascade);     

            // SensorData -> Alert (1..* | *..1)    
            builder.Entity<SensorData>()
                .HasMany(sd => sd.Alerts)         
                .WithOne(a => a.SensorData)        
                .HasForeignKey(a => a.DataId)      
                .OnDelete(DeleteBehavior.Cascade);
            
            // SensorData -> Diagnostic (1..1 | 1..1)    
            builder.Entity<SensorData>()
                .HasOne(sd => sd.Diagnostics)   
                .WithOne(d => d.SensorData)        
                .OnDelete(DeleteBehavior.Cascade);  
            
            // HeatmapChunk -> Feedback (1..* | 1..1)
            builder.Entity<HeatmapChunk>()
                .HasMany(hc => hc.Feedback)
                .WithOne(f => f.HeatmapChunk)
                .HasForeignKey(f => f.ChunkId)
                .OnDelete(DeleteBehavior.Cascade);
                
           // Feedback -> User (1..1 | 0..*)
           builder.Entity<ApplicationUser>()
               .HasMany(u => u.Feedback)    
               .WithOne(f => f.User)         
               .HasForeignKey(f => f.UserId)    
               .OnDelete(DeleteBehavior.Restrict);
           
           // Feedback -> FeedbackReply (1..* | *..1)
           builder.Entity<Feedback>()
               .HasMany(f => f.Replies)              
               .WithOne(fr => fr.Feedback)            
               .HasForeignKey(fr => fr.FeedbackId)    
               .OnDelete(DeleteBehavior.Cascade);
           
           
           // FeedbackReply -> User (1..1 | 0..*)
           builder.Entity<FeedbackReply>()
               .HasOne(fr => fr.User)                 
               .WithMany(u => u.FeedbackReplies)     
               .HasForeignKey(fr => fr.UserId)       
               .OnDelete(DeleteBehavior.Restrict);   
           
           // One Patient can have many PatientClinician links
           builder.Entity<ApplicationUser>()
               .HasMany(u => u.ClinicianLinks)  
               .WithOne(pc => pc.Patient)     
               .HasForeignKey(pc => pc.PatientId)  
               .OnDelete(DeleteBehavior.Restrict); 
           
            // One Clinician can have many PatientClinician links
           builder.Entity<ApplicationUser>()
               .HasMany(u => u.PatientLinks)    
               .WithOne(pc => pc.Clinician)  
               .HasForeignKey(pc => pc.ClinicianId) 
               .OnDelete(DeleteBehavior.Restrict); 
           
        }       
    }
}