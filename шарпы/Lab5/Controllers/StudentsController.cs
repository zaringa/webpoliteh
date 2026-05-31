using Lab5.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private static readonly List<Student> Students =
    [
        new Student
        {
            Id = 1,
            Name = "Ivan Petrov",
            Year = 2024,
            Courses =
            [
                new Course { Id = 1, Title = "Databases" },
                new Course { Id = 2, Title = "Algorithms" }
            ]
        },
        new Student
        {
            Id = 2,
            Name = "Anna Smirnova",
            Year = 2025,
            Courses =
            [
                new Course { Id = 1, Title = "Networks" },
                new Course { Id = 2, Title = "Web Development" }
            ]
        }
    ];

    private static int _nextId = Students.Max(student => student.Id) + 1;

    [HttpGet]
    public ActionResult<object> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string sort = "id",
        [FromQuery] int? year = null)
    {
        if (page < 1 || pageSize < 1)
        {
            return BadRequest("page и pageSize должны быть больше 0.");
        }

        var query = Students.AsQueryable();
        if (year.HasValue)
        {
            query = query.Where(student => student.Year == year.Value);
        }

        query = sort.ToLowerInvariant() switch
        {
            "name" => query.OrderBy(student => student.Name),
            "year" => query.OrderBy(student => student.Year),
            _ => query.OrderBy(student => student.Id)
        };

        var total = query.Count();
        var items = query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Ok(new
        {
            page,
            pageSize,
            total,
            sort,
            year,
            items
        });
    }

    [HttpGet("{id:int}")]
    public ActionResult<Student> GetById(int id)
    {
        var student = Students.FirstOrDefault(item => item.Id == id);
        return student is null ? NotFound() : Ok(student);
    }

    [HttpPost]
    public ActionResult<Student> Create([FromBody] StudentCreateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Year <= 0)
        {
            return BadRequest("Name обязателен, Year должен быть больше 0.");
        }

        var student = new Student
        {
            Id = _nextId++,
            Name = request.Name.Trim(),
            Year = request.Year,
            Courses = MapCourses(request.Courses)
        };

        Students.Add(student);
        return CreatedAtAction(nameof(GetById), new { id = student.Id }, student);
    }

    [HttpPut("{id:int}")]
    public ActionResult<Student> Update(int id, [FromBody] StudentUpdateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || request.Year <= 0)
        {
            return BadRequest("Name обязателен, Year должен быть больше 0.");
        }

        var existing = Students.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        existing.Name = request.Name.Trim();
        existing.Year = request.Year;
        existing.Courses = MapCourses(request.Courses);

        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = Students.FirstOrDefault(item => item.Id == id);
        if (existing is null)
        {
            return NotFound();
        }

        Students.Remove(existing);
        return NoContent();
    }

    [HttpGet("by-name/{name}")]
    public ActionResult<IEnumerable<Student>> GetByName(string name)
    {
        var matched = Students
            .Where(student => student.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return Ok(matched);
    }

    [HttpGet("by-year/{year:int}")]
    public ActionResult<IEnumerable<Student>> GetByYear(int year)
    {
        var matched = Students
            .Where(student => student.Year == year)
            .ToList();

        return Ok(matched);
    }

    [HttpGet("{id:int}/courses")]
    public ActionResult<IEnumerable<Course>> GetCoursesByStudentId(int id)
    {
        var student = Students.FirstOrDefault(item => item.Id == id);
        if (student is null)
        {
            return NotFound();
        }

        return Ok(student.Courses);
    }

    [HttpGet("profile/{id:int?}")]
    public ActionResult<object> GetProfile(int? id = null)
    {
        if (id is null)
        {
            var preview = Students
                .Select(student => new { student.Id, student.Name, student.Year })
                .ToList();

            return Ok(new
            {
                message = "id не указан. Возвращен список профилей.",
                items = preview
            });
        }

        var student = Students.FirstOrDefault(item => item.Id == id.Value);
        return student is null ? NotFound() : Ok(student);
    }

    private static List<Course> MapCourses(IEnumerable<string>? courseTitles)
    {
        if (courseTitles is null)
        {
            return new List<Course>();
        }

        var titles = courseTitles
            .Where(title => !string.IsNullOrWhiteSpace(title))
            .Select(title => title.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var courses = new List<Course>();
        for (var index = 0; index < titles.Count; index++)
        {
            courses.Add(new Course
            {
                Id = index + 1,
                Title = titles[index]
            });
        }

        return courses;
    }
}
