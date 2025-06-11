using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutoringBackend.Models;

public class Student
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public required string Name { get; set; }
    
    [Required]
    [EmailAddress]
    [StringLength(150)]
    public required string Email { get; set; }
    
    [StringLength(15)]
    public string? PhoneNumber { get; set; }
    
    [Range(5, 100)]
    public int? Age { get; set; }
    
    [StringLength(50)]
    public string? Grade { get; set; }
    
    [StringLength(200)]
    public string? Address { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public ICollection<StudentClassEnrollment> ClassEnrollments { get; set; } = new List<StudentClassEnrollment>();
    public ICollection<StudentScheduleEnrollment> ScheduleEnrollments { get; set; } = new List<StudentScheduleEnrollment>();
}
