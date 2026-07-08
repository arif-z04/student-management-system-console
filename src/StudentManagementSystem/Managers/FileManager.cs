using System.Text.Json;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Managers;


public sealed class FileManager
{
    private readonly string _baseDirectory;
    private readonly JsonSerializerOptions _jsonOptions;

    public FileManager(string baseDirectory)
    {
        _baseDirectory = baseDirectory;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

    }

    private string DataDir => Path.Combine(_baseDirectory, "Data");
    private string StudentPath => Path.Combine(DataDir, "student.json");
    private string CoursesPath => Path.Combine(DataDir, "courses.json");
    private string EnrollmentsPath => Path.Combine(DataDir, "enrollments.json");

    public List<Student> LoadStudents() => Load<List<Student>>(StudentPath) ?? new();

    public List<Course> LoadCourses() => Load<List<Course>>(CoursesPath) ?? new();

    public List<Enrollment> LoadEnrollments() => Load<List<Enrollment>>(EnrollmentsPath) ?? new();

    public void SaveStudents(List<Student> students) => Save(StudentPath, students);

    public void SaveCourses(List<Course> courses) => Save(CoursesPath, courses);

    public void SaveEnrollments(List<Enrollment> enrollments) => Save(CoursesPath, enrollments);

    private T? Load<T> (string path)
    {
        EnsureDataDirectory();

        if (!File.Exists(path))
        {
            var empty = Activator.CreateInstance<T>();
            Save(path, empty);
            return empty;
        }

        try
        {
           var json = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Activator.CreateInstance<T>();
            }
            return JsonSerializer.Deserialize<T>(json, _jsonOptions);
        }
        catch (JsonException ex)
        {
            var backupPath = $"{path}.corrupt.{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
            File.Copy(path, backupPath, overwrite:true);
            Save(path, Activator.CreateInstance<T>());

            throw new InvalidOperationException(
                $"JSON file '{Path.GetFileName(path)}' is corrupted. A backup was created: {Path.GetFileName(backupPath)}", ex
            );
        }
    }
    private void Save<T>(string path, T? data)
    {
        EnsureDataDirectory();

        try
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            File.WriteAllText(path, json);
        }
        catch(IOException ex)
        {
            throw new InvalidOperationException($"Failed writing '{Path.GetFileName(path)}'.", ex);
        }
        
    }

    private void EnsureDataDirectory()
    {
        if(!Directory.Exists(DataDir))
            Directory.CreateDirectory(DataDir);
    }

}