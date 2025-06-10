using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutoringBackend.Models;

public class Schedule
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int TutoringClassId { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    [Required]
    public DateTime EndTime { get; set; }
    
    [Required]
    [StringLength(20)]
    public required string DayOfWeek { get; set; } // Monday, Tuesday, etc.
    
    [StringLength(100)]
    public string? Location { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    [Range(1, 50)]
    public int MaxCapacity { get; set; } = 10;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
      // Navigation properties
    [JsonIgnore]
    public TutoringClass TutoringClass { get; set; } = null!;
    public ICollection<StudentScheduleEnrollment> StudentEnrollments { get; set; } = new List<StudentScheduleEnrollment>();
}
