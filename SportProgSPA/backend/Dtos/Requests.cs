namespace SportProg.Api.Dtos;

public record RegisterRequest(string Name, string Email, string Password);
public record LoginRequest(string Email, string Password);
public record UpdateProfileRequest(string Name, string City);
public record SubmitSolutionRequest(string Language, string Code);
