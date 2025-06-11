using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutoringBackend.Models;

public class TutoringClass
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [StringLength(500)]
    public string? Description { get; set; }
    
    [Required]
    [StringLength(50)]
    public required string Subject { get; set; }
    
    [Range(1, 5)]
    public int DifficultyLevel { get; set; } // 1-5 scale
    
    [Range(0, double.MaxValue)]
    public decimal PricePerHour { get; set; }
    
    public int MaxStudents { get; set; } = 10;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    public ICollection<StudentClassEnrollment> Enrollments { get; set; } = new List<StudentClassEnrollment>();
}
