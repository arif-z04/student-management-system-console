using StudentManagementSystem.Enums;
using StudentManagementSystem.Models;
using AppRepository = StudentManagementSystem.Repository.Repository;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem.Managers;

public sealed class StudentManager
{   
    public readonly AppRepository _repository;
    public StudentManager(AppRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<Student> GetAll()
        => _repository.Students.OrderBy(s => s.Id, StringComparer.OrdinalIgnoreCase).ToList();
    
    public Student? GetById(string id)
        => _repository.Students.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
    
    public void Add(Student student)
    {
        if(GetById(student.Id) is not null)
        {
            throw new InvalidCastException($"A student with ID '{student.Id}' already exists");
        }
        _repository.Students.Add(student);
    }


    public void Update(Student updated)
    {
        var existing = GetById(updated.Id) ?? throw new KeyNotFoundException($"Sutdent with ID '{updated.Id}' not found.");


        existing.FirstName   = updated.FirstName;
        existing.LastName    = updated.LastName;
        existing.DateOfBirth = updated.DateOfBirth;
        existing.Email       = updated.Email;
        existing.Phone       = updated.Phone;
        existing.Address     = updated.Address;
        existing.Department  = updated.Department;
        existing.Status      = updated.Status;
        existing.Gpa         = updated.Gpa;
        existing.UpdatedUtc  = DateTimeOffset.UtcNow;
    }

    public void Delete(string id)
    {
        var existing = GetById(id)?? throw new KeyNotFoundException($"Student with ID '{id}' not found.");
        _repository.Enrollments.RemoveAll(e => e.StudentId.Equals(id, StringComparison.OrdinalIgnoreCase));
        _repository.Students.Remove(existing);
    }

    public List<Student> Search(string query)
    {
        query = (query ?? string.Empty).Trim();
        if(query.Length == 0) return new();

        return _repository.Students
            .Where(s =>
                s.Id.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.LastName.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Email.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                s.Department.ToString().Contains(query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(s => s.FullName)
            .ToList();
    } 

    public (Student?  Highest, Student? Lowest, decimal Average) GetGpaStats()
    {
        if(_repository.Students.Count == 0)
        {
            return (null, null, 0m);
        }

        var highest = _repository.Students.OrderByDescending(s => s.Gpa).First();
        var lowest = _repository.Students.OrderBy(s => s.Gpa).First();
        var avg = _repository.Students.Average(s => s.Gpa);

        return (highest, lowest, decimal.Round((decimal)avg, 2));
    }

    public Dictionary<Department, int> GetDepartmentCounts()
        => _repository.Students
            .GroupBy(s => s.Department)
            .OrderBy(g => g.Key.ToString())
            .ToDictionary(g => g.Key, g => g.Count());

    public int GetStudentCount() => _repository.Students.Count;

    public static Student CreateValidatedStudent(
        string             id,
        string      firstName,
        string       lastName,
        DateOnly          dob,
        string          email,
        string          phone,
        Address       address,
        Department department,
        StudentStatus  status,
        decimal           gpa
    )
    {
        Validation.RequireStudentId(id);
        Validation.RequireName(firstName, "First Name");
        Validation.RequireName(lastName, "Last Name");
        Validation.RequireAge(dob, minAge: 10, maxAge: 80);
        Validation.RequireEmail(email);
        Validation.RequirePhone(phone);
        Validation.RequireGpa(gpa);

        return new Student(id, firstName, lastName, dob, email, phone, address, department, status, gpa);
    }    
}