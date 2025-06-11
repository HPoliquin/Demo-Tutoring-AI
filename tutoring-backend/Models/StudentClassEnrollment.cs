using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TutoringBackend.Models;

// Junction table for many-to-many relationship between Students and Classes
public class StudentClassEnrollment
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public int TutoringClassId { get; set; }
    
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    
    public bool IsActive { get; set; } = true;
      // Navigation properties
    [JsonIgnore]
    public Student Student { get; set; } = null!;
    [JsonIgnore]
    public TutoringClass TutoringClass { get; set; } = null!;
}
