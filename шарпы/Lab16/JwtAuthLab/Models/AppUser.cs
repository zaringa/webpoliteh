namespace JwtAuthLab.Models;

public record AppUser(
    string Id,
    string Login,
    string Password,
    string Role
);
