using StudentManagementSystem.Enums;
using StudentManagementSystem.Interfaces;

namespace StudentManagementSystem.Models;

public sealed class Course: IPrintable
{
    
    public Course(string code, string title, Department department, int creditHours)
    {
        Code = code;
        Title = title;
        Department = department;
        CreditHours = creditHours;
        CreatedUtc = DateTimeOffset.UtcNow;
    }
    public string Code{get; set;}
    public string Title{get; set;}
    public Department Department{ get; set; }
    public int CreditHours{ get; set; }
    public DateTimeOffset CreatedUtc {get; set;}
    public string[] ToRow() => [Code, Title, Department.ToString(), CreditHours.ToString()];
}