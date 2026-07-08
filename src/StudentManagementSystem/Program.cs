using StudentManagementSystem.Managers;
using AppRepository = StudentManagementSystem.Repository.Repository;
using StudentManagementSystem.Utilities;

namespace StudentManagementSystem;

internal static class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var theme = ColorTheme.ModernDark();
        ConsoleUI.ApplyTheme(theme);

        var fileManager = new FileManager(AppContext.BaseDirectory);
        var repository = new AppRepository(fileManager);
        var studentManager = new StudentManager(repository);
        var courseManager = new CourseManager(repository);
        var enrollmentManager = new EnrollmentManager(repository, studentManager, courseManager);

        // var app = new ConsoleUI(theme, repository, studentManager, courseManager, enrollmentManager);
        // app.Run();
        Console.WriteLine("It's working somehow.");
    }
}
