namespace SportProg.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string PasswordSalt { get; set; } = string.Empty;
    public string City { get; set; } = "Минск";
    public string AvatarColor { get; set; } = "#2563eb";
    public int Rating { get; set; } = 1000;
    public int SolvedCount { get; set; }
    public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

    public ICollection<FavoriteAlgorithm> Favorites { get; set; } = new List<FavoriteAlgorithm>();
    public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
}
