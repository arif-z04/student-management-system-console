using StudentManagementSystem.Models;
using AppRepository = StudentManagementSystem.Repository.Repository;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Managers;

public sealed class EnrollmentManager
{
    private readonly AppRepository _repository;
    private readonly StudentManager _studentManager;
    private readonly CourseManager _courseManager;

    public EnrollmentManager(AppRepository repository, StudentManager studentManager, CourseManager courseManager)
    {
        _repository = repository;
        _studentManager = studentManager;
        _courseManager = courseManager;
    }

    public IReadOnlyList<Enrollment> GetAll()
        => _repository.Enrollments.OrderBy(e => e.EnrolledOn).ThenBy(e => e.EnrollmentId).ToList();

    public IReadOnlyList<Enrollment> GetByStudent(string studentId)
        => _repository.Enrollments
            .Where(e => e.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(e => e.CourseCode)
            .ToList();

    public void Enroll(string studentId, string courseCode, DateOnly enrolledOn)
    {
        Validation.RequireStudentId(studentId);
        Validation.RequireCourseCode(courseCode);

        _ = _studentManager.GetById(studentId) ?? throw new KeyNotFoundException($"Student '{studentId}' not found.");
        _ = _courseManager.GetByCode(courseCode) ?? throw new KeyNotFoundException($"Course '{courseCode}' not found.");

        var already = _repository.Enrollments.Any(e =>
            e.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase) &&
            e.CourseCode.Equals(courseCode, StringComparison.OrdinalIgnoreCase));

        if (already)
            throw new InvalidOperationException($"Student '{studentId}' is already enrolled in '{courseCode}'.");

        var enrollmentId = GenerateEnrollmentId();
        _repository.Enrollments.Add(new Enrollment(enrollmentId, studentId, courseCode, enrolledOn));
    }

    public void Drop(string studentId, string courseCode)
    {
        var existing = _repository.Enrollments.FirstOrDefault(e =>
            e.StudentId.Equals(studentId, StringComparison.OrdinalIgnoreCase) &&
            e.CourseCode.Equals(courseCode, StringComparison.OrdinalIgnoreCase));

        if (existing is null)
            throw new KeyNotFoundException($"Enrollment not found for Student '{studentId}' and Course '{courseCode}'.");

        _repository.Enrollments.Remove(existing);
    }

    public List<Course> GetCoursesForStudent(string studentId)
    {
        var codes = GetByStudent(studentId).Select(e => e.CourseCode).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return _repository.Courses
            .Where(c => codes.Contains(c.Code))
            .OrderBy(c => c.Code)
            .ToList();
    }

    private string GenerateEnrollmentId()
    {
        var next = _repository.Enrollments.Count + 1;
        while (_repository.Enrollments.Any(e => e.EnrollmentId.Equals($"ENR-{next:0000}", StringComparison.OrdinalIgnoreCase)))
            next++;

        return $"ENR-{next:0000}";
    }
}
