using Microsoft.EntityFrameworkCore;
using TutoringBackend.Models;

namespace TutoringBackend.Database;

public class TutoringDbContext : DbContext
{
    public TutoringDbContext(DbContextOptions<TutoringDbContext> options) : base(options) { }

    public DbSet<Student> Students { get; set; }
    public DbSet<TutoringClass> TutoringClasses { get; set; }
    public DbSet<Schedule> Schedules { get; set; }
    public DbSet<StudentClassEnrollment> StudentClassEnrollments { get; set; }
    public DbSet<StudentScheduleEnrollment> StudentScheduleEnrollments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Student entity
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // Configure TutoringClass entity
        modelBuilder.Entity<TutoringClass>(entity =>
        {
            entity.Property(e => e.PricePerHour).HasPrecision(10, 2);
        });

        // Configure Schedule relationships
        modelBuilder.Entity<Schedule>(entity =>
        {
            entity.HasOne(s => s.TutoringClass)
                  .WithMany(tc => tc.Schedules)
                  .HasForeignKey(s => s.TutoringClassId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure StudentClassEnrollment relationships
        modelBuilder.Entity<StudentClassEnrollment>(entity =>
        {
            entity.HasOne(sce => sce.Student)
                  .WithMany(s => s.ClassEnrollments)
                  .HasForeignKey(sce => sce.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sce => sce.TutoringClass)
                  .WithMany(tc => tc.Enrollments)
                  .HasForeignKey(sce => sce.TutoringClassId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.StudentId, e.TutoringClassId }).IsUnique();
        });

        // Configure StudentScheduleEnrollment relationships
        modelBuilder.Entity<StudentScheduleEnrollment>(entity =>
        {
            entity.HasOne(sse => sse.Student)
                  .WithMany(s => s.ScheduleEnrollments)
                  .HasForeignKey(sse => sse.StudentId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(sse => sse.Schedule)
                  .WithMany(s => s.StudentEnrollments)
                  .HasForeignKey(sse => sse.ScheduleId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.StudentId, e.ScheduleId }).IsUnique();
        });
    }
}
