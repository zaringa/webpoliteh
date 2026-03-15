namespace Lab5.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<Course> Courses { get; set; } = new();
}

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
}

public class StudentCreateRequest
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<string> Courses { get; set; } = new();
}

public class StudentUpdateRequest
{
    public string Name { get; set; } = string.Empty;
    public int Year { get; set; }
    public List<string> Courses { get; set; } = new();
}
