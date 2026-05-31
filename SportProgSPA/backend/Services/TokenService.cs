using System.Text;
using SportProg.Api.Models;

namespace SportProg.Api.Services;

public class TokenService
{
    public string CreateToken(User user)
    {
        var payload = $"{user.Id}:{user.Email}:{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    public bool TryGetUserId(HttpRequest request, out int userId)
    {
        userId = 0;
        var header = request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        try
        {
            var token = header["Bearer ".Length..].Trim();
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var idPart = decoded.Split(':', 2)[0];
            return int.TryParse(idPart, out userId);
        }
        catch
        {
            return false;
        }
    }
}
