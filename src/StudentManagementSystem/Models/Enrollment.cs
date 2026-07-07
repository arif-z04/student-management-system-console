using StudentManagementSystem.Interfaces;

namespace StudentManagementSystem.Models;

public sealed class Enrollment: IPrintable
{
    public Enrollment(string enrollmentId, string studentId, string courseCode, DateOnly enrolledOn)
    {
        EnrollmentId = enrollmentId;
        StudentId    = studentId;
        CourseCode   = courseCode;
        EnrolledOn   = enrolledOn;
        CreatedUtc   = DateTimeOffset.UtcNow;
    }

    public string EnrollmentId { get; init;}
    public string StudentId {get; init;}
    public string CourseCode{get; init;}
    public DateOnly EnrolledOn{get; init;}
    public DateTimeOffset CreatedUtc{get; init;}
    public string[] ToRow() => [EnrollmentId, StudentId, CourseCode, EnrolledOn.ToString("yyyy-MM-dd")];
}