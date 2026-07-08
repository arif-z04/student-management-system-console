using System.Text.RegularExpressions;

namespace StudentManagementSystem.Utilities;

public static class Validation
{
    // Regex to validate information
    private static readonly Regex EmailRegex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
    private static readonly Regex PhoneRegex = new(@"^\+?[0-9][0-9\-\s]{6,}$", RegexOptions.Compiled);

    // Need to update the student id including session + department + studentID
    private static readonly Regex StudentIdRegex = new(@"^S-[0-9]{4}$", RegexOptions.Compiled);

    private static readonly Regex CourseCodeRegex = new(@"^[A-Z]{3}[0-9]{3}$", RegexOptions.Compiled);

    public static void RequireNonEmpty(string? value, string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{fieldName} is required.");
        }
    }

    public static void RequireName(string? value, string fieldName)
    {
        RequireNonEmpty(value, fieldName);
        if (value!.Trim().Length < 2)
        {
            throw new ArgumentException($"{fieldName} must be at least 2 characters.");
        }
    }

    public static void RequireEmail(string? email)
    {
        RequireNonEmpty(email, "Email");
        if (!EmailRegex.IsMatch(email!.Trim()))
        {
            throw new ArgumentException("Email format is invalid.");
        }
    }
    public static void RequirePhone(string? phone)
    {
        RequireNonEmpty(phone, "Phone");
        if (!PhoneRegex.IsMatch(phone!.Trim()))
        {
            throw new ArgumentException("Phone format is invalid");
        }
    }

    public static void RequireStudentId(string? id)
    {
        RequireNonEmpty(id, "Student ID");
        if (!StudentIdRegex.IsMatch(id!.Trim().ToUpperInvariant()))
        {
            throw new ArgumentException("Student ID must match format: S-0001");
        }
    }

    public static void RequireCourseCode(string? code)
    {
        RequireNonEmpty(code, "Course code");
        if (!CourseCodeRegex.IsMatch(code!.Trim().ToUpperInvariant()))
            throw new ArgumentException("Course code must match format: ABC123 (3 letters + 3 digits).");
    }

    public static void RequireGpa(decimal gpa)
    {
        if (gpa < 0m || gpa > 4m)
            throw new ArgumentOutOfRangeException(nameof(gpa), "GPA must be between 0.00 and 4.00.");
    }

    public static void RequireRange(int value, int min, int max, string fieldName)
    {
        if (value < min || value > max)
            throw new ArgumentOutOfRangeException(fieldName, $"{fieldName} must be between {min} and {max}.");
    }

    public static void RequireAge(DateOnly dob, int minAge, int maxAge)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var age = today.Year - dob.Year;
        if (today < dob.AddYears(age)) age--;

        if (age < minAge || age > maxAge)
            throw new ArgumentOutOfRangeException(nameof(dob), $"Age must be between {minAge} and {maxAge}.");
    }


}