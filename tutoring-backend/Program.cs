using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using TutoringBackend.Database;
using TutoringBackend.api;
using TutoringBackend.Models;


// Ensure the .env file is loaded for migrations
if (args.Contains("--migrate"))
{
    Env.Load();
}

// Load environment variables from the .env file
Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var configuration = builder.Configuration;
var sendGridApiKey = configuration["SendGrid:ApiKey"];

builder.Services.AddSingleton<ISendGridClient>(new SendGridClient(sendGridApiKey));

builder.Services.AddDbContext<TutoringDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Use CORS policy
app.UseCors("AllowAll");

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapPost("/send-email", async (ISendGridClient sendGridClient, EmailRequest emailRequest) =>
{
    var from = new EmailAddress("your-email@example.com", "Your Name");
    var subject = emailRequest.Subject;
    var to = new EmailAddress(emailRequest.To);
    var plainTextContent = emailRequest.PlainTextContent;
    var htmlContent = emailRequest.HtmlContent;
    var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

    var response = await sendGridClient.SendEmailAsync(msg);
    return Results.Ok(response.StatusCode);
});

// Add CRUD endpoints for Student
app.MapPost("/students", async (TutoringDbContext db, Student student) =>
{
    // Ensure DateTime values are properly set to UTC
    student.CreatedAt = DateTime.UtcNow;
    
    db.Students.Add(student);
    await db.SaveChangesAsync();
    return Results.Created($"/students/{student.Id}", student);
});

app.MapGet("/students", async (TutoringDbContext db) =>
{
    return Results.Ok(await db.Students.ToListAsync());
});

// Move the /students/mock route before /students/{id}
app.MapPost("/students/mock", async (TutoringDbContext db) =>
{
    var mockStudents = new List<Student>
    {
        new Student { Name = "Alice Johnson", Email = "alice.johnson@example.com", PhoneNumber = "555-0101", Age = 16, Grade = "10th", Address = "123 Main St" },
        new Student { Name = "Bob Smith", Email = "bob.smith@example.com", PhoneNumber = "555-0102", Age = 17, Grade = "11th", Address = "456 Oak Ave" },
        new Student { Name = "Charlie Brown", Email = "charlie.brown@example.com", PhoneNumber = "555-0103", Age = 15, Grade = "9th", Address = "789 Pine Rd" },
        new Student { Name = "Diana Prince", Email = "diana.prince@example.com", PhoneNumber = "555-0104", Age = 18, Grade = "12th", Address = "321 Elm St" },
        new Student { Name = "Edward Norton", Email = "edward.norton@example.com", PhoneNumber = "555-0105", Age = 16, Grade = "10th", Address = "654 Maple Dr" }
    };

    db.Students.AddRange(mockStudents);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.MapGet("/students/{id}", async (TutoringDbContext db, int id) =>
{
    var student = await db.Students.FindAsync(id);
    return student is not null ? Results.Ok(student) : Results.NotFound();
});

app.MapPut("/students/{id}", async (TutoringDbContext db, int id, Student updatedStudent) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null) return Results.NotFound();

    student.Name = updatedStudent.Name;
    student.Email = updatedStudent.Email;
    student.PhoneNumber = updatedStudent.PhoneNumber;
    student.Age = updatedStudent.Age;
    student.Grade = updatedStudent.Grade;
    student.Address = updatedStudent.Address;
    student.IsActive = updatedStudent.IsActive;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/students/{id}", async (TutoringDbContext db, int id) =>
{
    var student = await db.Students.FindAsync(id);
    if (student is null) return Results.NotFound();

    db.Students.Remove(student);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

// CRUD endpoints for TutoringClass
app.MapPost("/classes", async (TutoringDbContext db, TutoringClass tutoringClass) =>
{
    // Ensure DateTime values are properly set to UTC
    tutoringClass.CreatedAt = DateTime.UtcNow;
    
    db.TutoringClasses.Add(tutoringClass);
    await db.SaveChangesAsync();
    return Results.Created($"/classes/{tutoringClass.Id}", tutoringClass);
});

app.MapGet("/classes", async (TutoringDbContext db) =>
{
    var classes = await db.TutoringClasses
        .Include(tc => tc.Schedules)
        .Include(tc => tc.Enrollments)
            .ThenInclude(e => e.Student)
        .ToListAsync();
    return Results.Ok(classes);
});

app.MapGet("/classes/{id}", async (TutoringDbContext db, int id) =>
{
    var tutoringClass = await db.TutoringClasses
        .Include(tc => tc.Schedules)
        .Include(tc => tc.Enrollments)
            .ThenInclude(e => e.Student)
        .FirstOrDefaultAsync(tc => tc.Id == id);
    return tutoringClass is not null ? Results.Ok(tutoringClass) : Results.NotFound();
});

app.MapPut("/classes/{id}", async (TutoringDbContext db, int id, TutoringClass updatedClass) =>
{
    var tutoringClass = await db.TutoringClasses.FindAsync(id);
    if (tutoringClass is null) return Results.NotFound();

    tutoringClass.Name = updatedClass.Name;
    tutoringClass.Description = updatedClass.Description;
    tutoringClass.Subject = updatedClass.Subject;
    tutoringClass.DifficultyLevel = updatedClass.DifficultyLevel;
    tutoringClass.PricePerHour = updatedClass.PricePerHour;
    tutoringClass.MaxStudents = updatedClass.MaxStudents;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/classes/{id}", async (TutoringDbContext db, int id) =>
{
    var tutoringClass = await db.TutoringClasses.FindAsync(id);
    if (tutoringClass is null) return Results.NotFound();

    db.TutoringClasses.Remove(tutoringClass);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// CRUD endpoints for Schedule
app.MapPost("/schedules", async (TutoringDbContext db, Schedule schedule) =>
{
    // Ensure DateTime values are properly set to UTC
    schedule.StartTime = DateTime.SpecifyKind(schedule.StartTime, DateTimeKind.Utc);
    schedule.EndTime = DateTime.SpecifyKind(schedule.EndTime, DateTimeKind.Utc);
    schedule.CreatedAt = DateTime.UtcNow;
    
    db.Schedules.Add(schedule);
    await db.SaveChangesAsync();
    return Results.Created($"/schedules/{schedule.Id}", schedule);
});

app.MapGet("/schedules", async (TutoringDbContext db) =>
{
    var schedules = await db.Schedules
        .Include(s => s.TutoringClass)
        .Include(s => s.StudentEnrollments)
            .ThenInclude(se => se.Student)
        .ToListAsync();
    return Results.Ok(schedules);
});

app.MapGet("/schedules/{id}", async (TutoringDbContext db, int id) =>
{
    var schedule = await db.Schedules
        .Include(s => s.TutoringClass)
        .Include(s => s.StudentEnrollments)
            .ThenInclude(se => se.Student)
        .FirstOrDefaultAsync(s => s.Id == id);
    return schedule is not null ? Results.Ok(schedule) : Results.NotFound();
});

app.MapPut("/schedules/{id}", async (TutoringDbContext db, int id, Schedule updatedSchedule) =>
{
    var schedule = await db.Schedules.FindAsync(id);
    if (schedule is null) return Results.NotFound();

    schedule.TutoringClassId = updatedSchedule.TutoringClassId;
    schedule.StartTime = DateTime.SpecifyKind(updatedSchedule.StartTime, DateTimeKind.Utc);
    schedule.EndTime = DateTime.SpecifyKind(updatedSchedule.EndTime, DateTimeKind.Utc);
    schedule.DayOfWeek = updatedSchedule.DayOfWeek;
    schedule.Location = updatedSchedule.Location;
    schedule.IsActive = updatedSchedule.IsActive;
    schedule.MaxCapacity = updatedSchedule.MaxCapacity;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/schedules/{id}", async (TutoringDbContext db, int id) =>
{
    var schedule = await db.Schedules.FindAsync(id);
    if (schedule is null) return Results.NotFound();

    db.Schedules.Remove(schedule);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Enrollment endpoints
app.MapPost("/enrollments/class", async (TutoringDbContext db, StudentClassEnrollment enrollment) =>
{
    // Check if student is already enrolled in this class
    var existingEnrollment = await db.StudentClassEnrollments
        .FirstOrDefaultAsync(e => e.StudentId == enrollment.StudentId && e.TutoringClassId == enrollment.TutoringClassId);
    
    if (existingEnrollment is not null)
        return Results.BadRequest("Student is already enrolled in this class");

    // Ensure DateTime values are properly set to UTC
    enrollment.EnrolledAt = DateTime.UtcNow;

    db.StudentClassEnrollments.Add(enrollment);
    await db.SaveChangesAsync();
    return Results.Created($"/enrollments/class/{enrollment.Id}", enrollment);
});

app.MapPost("/enrollments/schedule", async (TutoringDbContext db, StudentScheduleEnrollment enrollment) =>
{
    // Check if student is already enrolled in this schedule
    var existingEnrollment = await db.StudentScheduleEnrollments
        .FirstOrDefaultAsync(e => e.StudentId == enrollment.StudentId && e.ScheduleId == enrollment.ScheduleId);
    
    if (existingEnrollment is not null)
        return Results.BadRequest("Student is already enrolled in this schedule");

    // Check schedule capacity
    var schedule = await db.Schedules
        .Include(s => s.StudentEnrollments)
        .FirstOrDefaultAsync(s => s.Id == enrollment.ScheduleId);
    
    if (schedule is null)
        return Results.NotFound("Schedule not found");
    
    if (schedule.StudentEnrollments.Count >= schedule.MaxCapacity)
        return Results.BadRequest("Schedule is at full capacity");

    // Ensure DateTime values are properly set to UTC
    enrollment.EnrolledAt = DateTime.UtcNow;

    db.StudentScheduleEnrollments.Add(enrollment);
    await db.SaveChangesAsync();
    return Results.Created($"/enrollments/schedule/{enrollment.Id}", enrollment);
});

// Add mock data endpoints
app.MapPost("/classes/mock", async (TutoringDbContext db) =>
{
    var mockClasses = new List<TutoringClass>
    {
        new TutoringClass { Name = "Advanced Mathematics", Description = "Calculus and advanced algebra", Subject = "Mathematics", DifficultyLevel = 4, PricePerHour = 50.00m, MaxStudents = 8 },
        new TutoringClass { Name = "Basic English", Description = "Grammar and writing fundamentals", Subject = "English", DifficultyLevel = 2, PricePerHour = 35.00m, MaxStudents = 12 },
        new TutoringClass { Name = "Physics Fundamentals", Description = "Introduction to mechanics and thermodynamics", Subject = "Physics", DifficultyLevel = 3, PricePerHour = 45.00m, MaxStudents = 10 },
        new TutoringClass { Name = "Computer Programming", Description = "Learn C# and .NET development", Subject = "Computer Science", DifficultyLevel = 4, PricePerHour = 60.00m, MaxStudents = 6 },
        new TutoringClass { Name = "Spanish Conversation", Description = "Practice speaking and listening", Subject = "Spanish", DifficultyLevel = 2, PricePerHour = 40.00m, MaxStudents = 15 }
    };

    db.TutoringClasses.AddRange(mockClasses);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPost("/schedules/mock", async (TutoringDbContext db) =>
{
    var classes = await db.TutoringClasses.ToListAsync();
    if (!classes.Any()) return Results.BadRequest("No classes found. Add classes first.");

    var mockSchedules = new List<Schedule>();
    var startDate = DateTime.UtcNow.Date.AddDays(7); // Start next week in UTC

    foreach (var tutoringClass in classes)
    {
        // Add multiple schedules per class
        mockSchedules.AddRange(new[]
        {
            new Schedule 
            { 
                TutoringClassId = tutoringClass.Id, 
                StartTime = DateTime.SpecifyKind(startDate.AddHours(9), DateTimeKind.Utc), 
                EndTime = DateTime.SpecifyKind(startDate.AddHours(10), DateTimeKind.Utc), 
                DayOfWeek = "Monday", 
                Location = "Room A1",
                IsActive = true,
                MaxCapacity = tutoringClass.MaxStudents,
                CreatedAt = DateTime.UtcNow
            },
            new Schedule 
            { 
                TutoringClassId = tutoringClass.Id, 
                StartTime = DateTime.SpecifyKind(startDate.AddDays(2).AddHours(14), DateTimeKind.Utc), 
                EndTime = DateTime.SpecifyKind(startDate.AddDays(2).AddHours(15), DateTimeKind.Utc), 
                DayOfWeek = "Wednesday", 
                Location = "Room B2",
                IsActive = true,
                MaxCapacity = tutoringClass.MaxStudents,
                CreatedAt = DateTime.UtcNow
            },
            new Schedule 
            { 
                TutoringClassId = tutoringClass.Id, 
                StartTime = DateTime.SpecifyKind(startDate.AddDays(4).AddHours(10), DateTimeKind.Utc), 
                EndTime = DateTime.SpecifyKind(startDate.AddDays(4).AddHours(11), DateTimeKind.Utc), 
                DayOfWeek = "Friday", 
                Location = "Room C3",
                IsActive = true,
                MaxCapacity = tutoringClass.MaxStudents,
                CreatedAt = DateTime.UtcNow
            }
        });
    }

    db.Schedules.AddRange(mockSchedules);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

await app.RunAsync();
