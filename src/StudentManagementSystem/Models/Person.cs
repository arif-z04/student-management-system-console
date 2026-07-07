
namespace StudentManagementSystem.Models;

public abstract class Person
{
    
    protected Person(string id, string firstName, string lastName, DateOnly dateOfBirth, string email, string phone, Address address)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Email = email;
        Phone = phone;
        Address = address;
    }
    public string Id {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string Email {get; set;}
    public DateOnly DateOfBirth {get; set;}
    public string Phone {get; set;}
    public Address Address {get; set;}

    public string FullName => $"{FirstName} {LastName}".Trim();

    public int Age
    {
        get
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - DateOfBirth.Year;
            if(today < DateOfBirth.AddYears(age)) age--;
            return age;
        }
    }

}