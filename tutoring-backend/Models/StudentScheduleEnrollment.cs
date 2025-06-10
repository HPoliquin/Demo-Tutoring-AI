using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutoringBackend.Models;

// Junction table for Students enrolled in specific Schedule sessions
public class StudentScheduleEnrollment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public int ScheduleId { get; set; }
    
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    [StringLength(500)]
    public string? Notes { get; set; }
      // Navigation properties
    [JsonIgnore]
    public Student Student { get; set; } = null!;
    [JsonIgnore]
    public Schedule Schedule { get; set; } = null!;
}
