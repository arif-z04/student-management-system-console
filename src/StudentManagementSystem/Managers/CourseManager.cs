using StudentManagementSystem.Enums;
using StudentManagementSystem.Models;
using StudentManagementSystem.Utilities;

using AppRepository = StudentManagementSystem.Repository.Repository;


public sealed class CourseManager
{
    private readonly AppRepository _repository;

    public CourseManager(AppRepository repository)
    {
        _repository = repository;
    }


    public IReadOnlyList<Course> GetAll()
        => _repository.Courses.OrderBy(c => c.Code, StringComparer.OrdinalIgnoreCase).ToList();

    public Course? GetByCode(string code)
        => _repository.Courses.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public void Add(Course course)
    {
        if (GetByCode(course.Code) is not null)
            throw new InvalidOperationException($"Course '{course.Code}' already exists.");

        _repository.Courses.Add(course);
    }

    public void Remove(string code)
    {
        var existing = GetByCode(code) ?? throw new KeyNotFoundException($"Course '{code}' not found.");
        _repository.Enrollments.RemoveAll(e => e.CourseCode.Equals(code, StringComparison.OrdinalIgnoreCase));
        _repository.Courses.Remove(existing);
    }

    public static Course CreateValidatedCourse(string code, string title, Department department, int creditHours)
    {
        Validation.RequireCourseCode(code);
        Validation.RequireNonEmpty(title, "Course title");
        Validation.RequireRange(creditHours, 1, 5, "Credit hours");

        return new Course(code, title.Trim(), department, creditHours);
    }
}