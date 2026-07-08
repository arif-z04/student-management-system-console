using StudentManagementSystem.Enums;
using StudentManagementSystem.Managers;
using StudentManagementSystem.Models;
using AppRepository = StudentManagementSystem.Repository.Repository;

namespace StudentManagementSystem.Utilities;

public sealed class ConsoleUI
{
    private readonly ColorTheme _theme;
    private readonly AppRepository _repository;
    private readonly StudentManager _studentManager;
    private readonly CourseManager _courseManager;
    private readonly EnrollmentManager _enrollmentManager;

    private string _lastStatus = "Ready";

    public ConsoleUI(ColorTheme theme, AppRepository repository, StudentManager studentManager, CourseManager courseManager, EnrollmentManager enrollmentManager)
    {
        _theme = theme;
        _repository = repository;
        _studentManager = studentManager;
        _courseManager = courseManager;
        _enrollmentManager = enrollmentManager;

        _repository.RepositoryChanged += (_, e) =>
        {
            _lastStatus = $"{e.Message} (UTC {e.UtcTimestamp:HH:mm:ss})";
        };
    }

    public static void ApplyTheme(ColorTheme theme)
    {
        Console.BackgroundColor = theme.Background;
        Console.ForegroundColor = theme.Foreground;
    }

    public void Run()
    {
        ConsoleHelper.ClearAndReset(_theme);

        try
        {
            ConsoleHelper.WithSpinner(_theme, "Loading data...", () => _repository.LoadAll());
            _lastStatus = "Loaded data";
        }
        catch (Exception ex)
        {
            ShowError(_theme, ex.Message);
            _repository.Students = new();
            _repository.Courses = new();
            _repository.Enrollments = new();
            _lastStatus = "Started with empty data (load failed)";
            ConsoleHelper.Pause(_theme);
        }

        while (true)
        {
            var choice = MenuRenderer.RenderMenu(
                _theme,
                "Student Management System",
                [
                    "Student Management",
                    "Course Management",
                    "Enrollment",
                    "Reports",
                    "Save",
                    "Load",
                    "Exit"
                ]);

            switch (choice)
            {
                case 0:
                    StudentMenu();
                    break;
                case 1:
                    CourseMenu();
                    break;
                case 2:
                    EnrollmentMenu();
                    break;
                case 3:
                    ReportsMenu();
                    break;
                case 4:
                    Save();
                    break;
                case 5:
                    Load();
                    break;
                case 6:
                    if (ConsoleHelper.Confirm("Exit the application?", _theme))
                        return;
                    break;
            }
        }
    }

    public static void ShowError(ColorTheme theme, string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = theme.Error;
        Console.WriteLine($"[ERROR] {message}");
        Console.ForegroundColor = old;
    }

    public static void ShowSuccess(ColorTheme theme, string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = theme.Success;
        Console.WriteLine($"[OK] {message}");
        Console.ForegroundColor = old;
    }

    public static void ShowWarning(ColorTheme theme, string message)
    {
        var old = Console.ForegroundColor;
        Console.ForegroundColor = theme.Warning;
        Console.WriteLine($"[WARN] {message}");
        Console.ForegroundColor = old;
    }

    private void StudentMenu()
    {
        while (true)
        {
            var choice = MenuRenderer.RenderMenu(
                _theme,
                "Student Management",
                [
                    "Add Student",
                    "Update Student",
                    "Delete Student",
                    "Search Student",
                    "View Student",
                    "View All Students",
                    "Back"
                ]);

            try
            {
                switch (choice)
                {
                    case 0: AddStudent(); break;
                    case 1: UpdateStudent(); break;
                    case 2: DeleteStudent(); break;
                    case 3: SearchStudent(); break;
                    case 4: ViewStudent(); break;
                    case 5: ViewAllStudents(); break;
                    case 6: return;
                }
            }
            catch (Exception ex)
            {
                ShowError(_theme, ex.Message);
                ConsoleHelper.Pause(_theme);
            }
        }
    }

    private void CourseMenu()
    {
        while (true)
        {
            var choice = MenuRenderer.RenderMenu(
                _theme,
                "Course Management",
                [
                    "Add Course",
                    "Remove Course",
                    "View Courses",
                    "Back"
                ]);

            try
            {
                switch (choice)
                {
                    case 0: AddCourse(); break;
                    case 1: RemoveCourse(); break;
                    case 2: ViewCourses(); break;
                    case 3: return;
                }
            }
            catch (Exception ex)
            {
                ShowError(_theme, ex.Message);
                ConsoleHelper.Pause(_theme);
            }
        }
    }

    private void EnrollmentMenu()
    {
        while (true)
        {
            var choice = MenuRenderer.RenderMenu(
                _theme,
                "Enrollment",
                [
                    "Enroll Student",
                    "Drop Course",
                    "Student Course List",
                    "Back"
                ]);

            try
            {
                switch (choice)
                {
                    case 0: EnrollStudent(); break;
                    case 1: DropCourse(); break;
                    case 2: StudentCourseList(); break;
                    case 3: return;
                }
            }
            catch (Exception ex)
            {
                ShowError(_theme, ex.Message);
                ConsoleHelper.Pause(_theme);
            }
        }
    }

    private void ReportsMenu()
    {
        while (true)
        {
            var choice = MenuRenderer.RenderMenu(
                _theme,
                "Reports",
                [
                    "Highest GPA",
                    "Lowest GPA",
                    "Average GPA",
                    "Department Statistics",
                    "Student Count",
                    "Back"
                ]);

            switch (choice)
            {
                case 0: ShowHighestGpa(); break;
                case 1: ShowLowestGpa(); break;
                case 2: ShowAverageGpa(); break;
                case 3: ShowDepartmentStats(); break;
                case 4: ShowStudentCount(); break;
                case 5: return;
            }
        }
    }

    private void Save()
    {
        try
        {
            ConsoleHelper.WithSpinner(_theme, "Saving...", () => _repository.SaveAll());
            _lastStatus = "Saved data";
            ShowSuccess(_theme, "Data saved successfully.");
        }
        catch (Exception ex)
        {
            ShowError(_theme, ex.Message);
        }

        ConsoleHelper.Pause(_theme);
    }

    private void Load()
    {
        try
        {
            ConsoleHelper.WithSpinner(_theme, "Loading...", () => _repository.LoadAll());
            _lastStatus = "Loaded data";
            ShowSuccess(_theme, "Data loaded successfully.");
        }
        catch (Exception ex)
        {
            ShowError(_theme, ex.Message);
        }

        ConsoleHelper.Pause(_theme);
    }

    private void AddStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Add Student", _theme);

        var id = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID (S-0001): ", _theme));
        var firstName = InputHelper.ReadRequired("First name: ", _theme);
        var lastName = InputHelper.ReadRequired("Last name: ", _theme);
        var dob = InputHelper.ReadDate("Date of birth (YYYY-MM-DD): ", _theme);
        var email = InputHelper.ReadRequired("Email: ", _theme);
        var phone = InputHelper.ReadRequired("Phone: ", _theme);

        var street = InputHelper.ReadRequired("Street: ", _theme);
        var city = InputHelper.ReadRequired("City: ", _theme);
        var village = InputHelper.ReadRequired("Village: ", _theme);
        var postal = InputHelper.ReadRequired("Postal code: ", _theme);
        var country = InputHelper.ReadRequired("Country: ", _theme);

        var department = InputHelper.ReadEnum<Department>("Department:", _theme);
        var status = InputHelper.ReadEnum<StudentStatus>("Status:", _theme);
        var gpa = InputHelper.ReadDecimal("GPA (0.00 - 4.00): ", _theme, 0m, 4m);

        var student = StudentManager.CreateValidatedStudent(
            id, firstName, lastName, dob, email, phone,
            new Address(street, city, village, postal, country),
            department, status, gpa);

        _studentManager.Add(student);
        _lastStatus = $"Added student {student.Id}";
        ShowSuccess(_theme, $"Student '{student.Id}' added.");
        ConsoleHelper.Pause(_theme);
    }

    private void UpdateStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Update Student", _theme);

        var id = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID to update: ", _theme));
        var existing = _studentManager.GetById(id);

        if (existing is null)
        {
            ShowWarning(_theme, $"Student '{id}' not found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        Console.WriteLine("Leave blank to keep current value.");
        Console.WriteLine();

        var firstName = InputHelper.ReadOptional($"First name ({existing.FirstName}): ", _theme, existing.FirstName);
        var lastName = InputHelper.ReadOptional($"Last name ({existing.LastName}): ", _theme, existing.LastName);

        Console.ForegroundColor = _theme.Accent;
        Console.Write($"Date of birth ({existing.DateOfBirth:yyyy-MM-dd}): ");
        Console.ForegroundColor = _theme.Foreground;
        var dobRaw = Console.ReadLine();
        var dob = existing.DateOfBirth;
        if (!string.IsNullOrWhiteSpace(dobRaw))
        {
            if (!DateOnly.TryParse(dobRaw.Trim(), out dob))
                throw new ArgumentException("Invalid date format.");
        }

        var email = InputHelper.ReadOptional($"Email ({existing.Email}): ", _theme, existing.Email);
        var phone = InputHelper.ReadOptional($"Phone ({existing.Phone}): ", _theme, existing.Phone);
        var street = InputHelper.ReadOptional($"Street ({existing.Address.Street}): ", _theme, existing.Address.Street);
        var city = InputHelper.ReadOptional($"City ({existing.Address.City}): ", _theme, existing.Address.City);
        var village = InputHelper.ReadOptional($"Village ({existing.Address.Village}): ", _theme, existing.Address.Village);
        var postal = InputHelper.ReadOptional($"Postal ({existing.Address.PostalCode}): ", _theme, existing.Address.PostalCode);
        var country = InputHelper.ReadOptional($"Country ({existing.Address.Country}): ", _theme, existing.Address.Country);
        var department = InputHelper.ReadEnum<Department>($"Department (current: {existing.Department}):", _theme);
        var status = InputHelper.ReadEnum<StudentStatus>($"Status (current: {existing.Status}):", _theme);
        var gpa = InputHelper.ReadDecimal($"GPA (current: {existing.Gpa:0.00}): ", _theme, 0m, 4m);

        var updated = StudentManager.CreateValidatedStudent(
            existing.Id, firstName, lastName, dob, email, phone,
            new Address(street, city, village, postal, country),
            department, status, gpa);

        _studentManager.Update(updated);
        _lastStatus = $"Updated student {id}";
        ShowSuccess(_theme, $"Student '{id}' updated.");
        ConsoleHelper.Pause(_theme);
    }

    private void DeleteStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Delete Student", _theme);

        var id = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID to delete: ", _theme));
        var existing = _studentManager.GetById(id);
        if (existing is null)
        {
            ShowWarning(_theme, $"Student '{id}' not found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        if (!ConsoleHelper.Confirm($"Delete student '{existing.FullName}' ({existing.Id})? Enrollments will also be removed.", _theme))
            return;

        _studentManager.Delete(id);
        _lastStatus = $"Deleted student {id}";
        ShowSuccess(_theme, $"Student '{id}' deleted.");
        ConsoleHelper.Pause(_theme);
    }

    private void SearchStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Search Student", _theme);

        var query = InputHelper.ReadRequired("Search by ID/Name/Email/Department: ", _theme);
        var results = _studentManager.Search(query);

        if (results.Count == 0)
        {
            ShowWarning(_theme, "No students matched your search.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["ID", "Name", "Age", "GPA", "Department", "Status"], results.Select(s => s.ToRow()));
        ConsoleHelper.Pause(_theme);
    }

    private void ViewStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("View Student", _theme);

        var id = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID: ", _theme));
        var student = _studentManager.GetById(id);

        if (student is null)
        {
            ShowWarning(_theme, $"Student '{id}' not found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["ID", "Name", "Age", "GPA", "Department", "Status"], [student.ToRow()]);

        Console.WriteLine();
        Console.ForegroundColor = _theme.Muted;
        Console.WriteLine("Contact & Address");
        Console.ForegroundColor = _theme.Foreground;
        Console.WriteLine($"Email   : {student.Email}");
        Console.WriteLine($"Phone   : {student.Phone}");
        Console.WriteLine($"Address : {student.Address}");

        ConsoleHelper.Pause(_theme);
    }

    private void ViewAllStudents()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("All Students", _theme);

        var all = _studentManager.GetAll();
        if (all.Count == 0)
        {
            ShowWarning(_theme, "No students found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["ID", "Name", "Age", "GPA", "Department", "Status"], all.Select(s => s.ToRow()));
        ConsoleHelper.Pause(_theme);
    }

    private void AddCourse()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Add Course", _theme);

        var code = InputHelper.NormalizeCourseCode(InputHelper.ReadRequired("Course code (ABC123): ", _theme));
        var title = InputHelper.ReadRequired("Title: ", _theme);
        var dept = InputHelper.ReadEnum<Department>("Department:", _theme);
        var credits = InputHelper.ReadInt("Credit hours (1-5): ", _theme, 1, 5);

        var course = CourseManager.CreateValidatedCourse(code, title, dept, credits);
        _courseManager.Add(course);

        _lastStatus = $"Added course {course.Code}";
        ShowSuccess(_theme, $"Course '{course.Code}' added.");
        ConsoleHelper.Pause(_theme);
    }

    private void RemoveCourse()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Remove Course", _theme);

        var code = InputHelper.NormalizeCourseCode(InputHelper.ReadRequired("Course code to remove: ", _theme));
        var existing = _courseManager.GetByCode(code);

        if (existing is null)
        {
            ShowWarning(_theme, $"Course '{code}' not found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        if (!ConsoleHelper.Confirm($"Remove course '{existing.Code} - {existing.Title}'? Enrollments will also be removed.", _theme))
            return;

        _courseManager.Remove(code);
        _lastStatus = $"Removed course {code}";
        ShowSuccess(_theme, $"Course '{code}' removed.");
        ConsoleHelper.Pause(_theme);
    }

    private void ViewCourses()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Courses", _theme);

        var courses = _courseManager.GetAll();
        if (courses.Count == 0)
        {
            ShowWarning(_theme, "No courses found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["Code", "Title", "Department", "Credits"], courses.Select(c => c.ToRow()));
        ConsoleHelper.Pause(_theme);
    }

    private void EnrollStudent()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Enroll Student", _theme);

        var studentId = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID (S-0001): ", _theme));
        var courseCode = InputHelper.NormalizeCourseCode(InputHelper.ReadRequired("Course code (ABC123): ", _theme));
        var date = InputHelper.ReadDate("Enroll date (YYYY-MM-DD): ", _theme);

        _enrollmentManager.Enroll(studentId, courseCode, date);

        _lastStatus = $"Enrolled {studentId} in {courseCode}";
        ShowSuccess(_theme, "Enrollment created.");
        ConsoleHelper.Pause(_theme);
    }

    private void DropCourse()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Drop Course", _theme);

        var studentId = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID: ", _theme));
        var courseCode = InputHelper.NormalizeCourseCode(InputHelper.ReadRequired("Course code: ", _theme));

        _enrollmentManager.Drop(studentId, courseCode);

        _lastStatus = $"Dropped {studentId} from {courseCode}";
        ShowSuccess(_theme, "Course dropped.");
        ConsoleHelper.Pause(_theme);
    }

    private void StudentCourseList()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Student Course List", _theme);

        var studentId = InputHelper.NormalizeStudentId(InputHelper.ReadRequired("Student ID: ", _theme));
        var student = _studentManager.GetById(studentId);

        if (student is null)
        {
            ShowWarning(_theme, $"Student '{studentId}' not found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        var courses = _enrollmentManager.GetCoursesForStudent(studentId);

        Console.ForegroundColor = _theme.Muted;
        Console.WriteLine($"Student: {student.FullName} ({student.Id})");
        Console.ForegroundColor = _theme.Foreground;
        Console.WriteLine();

        if (courses.Count == 0)
        {
            ShowWarning(_theme, "No enrolled courses.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["Code", "Title", "Department", "Credits"], courses.Select(c => c.ToRow()));
        ConsoleHelper.Pause(_theme);
    }

    private void ShowHighestGpa()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Highest GPA", _theme);

        var (highest, _, _) = _studentManager.GetGpaStats();
        if (highest is null)
        {
            ShowWarning(_theme, "No students found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["ID", "Name", "Age", "GPA", "Department", "Status"], [highest.ToRow()]);
        ConsoleHelper.Pause(_theme);
    }

    private void ShowLowestGpa()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Lowest GPA", _theme);

        var (_, lowest, _) = _studentManager.GetGpaStats();
        if (lowest is null)
        {
            ShowWarning(_theme, "No students found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["ID", "Name", "Age", "GPA", "Department", "Status"], [lowest.ToRow()]);
        ConsoleHelper.Pause(_theme);
    }

    private void ShowAverageGpa()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Average GPA", _theme);

        var (_, _, avg) = _studentManager.GetGpaStats();
        Console.ForegroundColor = _theme.Accent;
        Console.WriteLine($"Average GPA: {avg:0.00}");
        Console.ForegroundColor = _theme.Foreground;

        ConsoleHelper.Pause(_theme);
    }

    private void ShowDepartmentStats()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Department Statistics", _theme);

        var stats = _studentManager.GetDepartmentCounts();
        if (stats.Count == 0)
        {
            ShowWarning(_theme, "No students found.");
            ConsoleHelper.Pause(_theme);
            return;
        }

        TablePrinter.Print(_theme, ["Department", "Student Count"], stats.Select(kvp => new[] { kvp.Key.ToString(), kvp.Value.ToString() }));
        ConsoleHelper.Pause(_theme);
    }

    private void ShowStudentCount()
    {
        ConsoleHelper.ClearAndReset(_theme);
        ConsoleHelper.DrawHeaderBox("Student Count", _theme);

        var count = _studentManager.GetStudentCount();
        Console.ForegroundColor = _theme.Accent;
        Console.WriteLine($"Total Students: {count}");
        Console.ForegroundColor = _theme.Foreground;

        ConsoleHelper.Pause(_theme);
    }
}
