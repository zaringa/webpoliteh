using System.Security.Cryptography;
using System.Text;

namespace SportProg.Api.Services;

public class PasswordService
{
    public (string Hash, string Salt) CreateHash(string password)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var salt = Convert.ToBase64String(saltBytes);
        return (ComputeHash(password, salt), salt);
    }

    public bool Verify(string password, string hash, string salt)
    {
        return ComputeHash(password, salt) == hash;
    }

    private static string ComputeHash(string password, string salt)
    {
        var bytes = Encoding.UTF8.GetBytes($"{salt}:{password}");
        return Convert.ToBase64String(SHA256.HashData(bytes));
    }
}
