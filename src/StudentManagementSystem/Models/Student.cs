using StudentManagementSystem.Enums;
using StudentManagementSystem.Interfaces;

namespace StudentManagementSystem.Models;


public sealed class Student : Person, IPrintable
{
    public Student(
        string id,
        string firstName,
        string lastName,
        DateOnly dateOfBirth,
        string email,
        string phone,
        Address address,
        Department department,
        StudentStatus status,
        decimal gpa
    ) : base(id, firstName, lastName, dateOfBirth, email, phone, address)
    {
        Department = department;
        Status = status;
        Gpa = gpa;
        CreatedUtc = DateTimeOffset.UtcNow;
    }


    public Department Department {get; set;}
    public StudentStatus Status {get; set;}
    public decimal Gpa{ get; set;}
    public DateTimeOffset CreatedUtc{ get; set;}
    public DateTimeOffset? UpdatedUtc{ get; set;}

    public string[] ToRow() 
        => [Id, FullName, Age.ToString(), Gpa.ToString("0.00"), Department.ToString(), Status.ToString()];
}