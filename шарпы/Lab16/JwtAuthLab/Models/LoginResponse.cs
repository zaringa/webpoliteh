namespace JwtAuthLab.Models;

public record LoginResponse(
    string Token,
    string UserId,
    string Login,
    string Role
);
