using StudentManagementSystem.Managers;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Repository;

public sealed class Repository
{
    private readonly FileManager _fileManager;

    public Repository(FileManager fileManager)
    {
        _fileManager = fileManager;

    }

    
    public List<Student> Students {get; set;} = new();
    public List<Course> Courses {get; set;} = new();
    public List<Enrollment> Enrollments {get; set;} = new();

    public event EventHandler<RepositoryChangedEventArgs>? RepositoryChanged;
    public void LoadAll()
    {
        Students = _fileManager.LoadStudents();
        Courses = _fileManager.LoadCourses();
        Enrollments = _fileManager.LoadEnrollments();
        OnRepositoryChanged("Loaded data from JSON files.");
    }

    public void SaveAll()
    {
        _fileManager.SaveStudents(Students);
        _fileManager.SaveCourses(Courses);
        _fileManager.SaveEnrollments(Enrollments);
        OnRepositoryChanged("Loaded data from JSON files.");
    }

    private void OnRepositoryChanged(string message)
    => RepositoryChanged?.Invoke(this, new RepositoryChangedEventArgs(message, DateTimeOffset.UtcNow));
}

public sealed class RepositoryChangedEventArgs : EventArgs
{
    public RepositoryChangedEventArgs(string message, DateTimeOffset utcTimestamp)
    {
        Message = message;
        UtcTimestamp = utcTimestamp;
    }

    public string Message {get;}
    public DateTimeOffset UtcTimestamp{ get; }
}