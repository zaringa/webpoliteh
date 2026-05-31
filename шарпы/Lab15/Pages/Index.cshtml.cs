using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StateLab.Pages;

public class IndexModel : PageModel
{
    private const string UserCookieKey = "StateLab.UserName";
    private const string UserSessionKey = "StateLab.UserName";
    private const string VisitCountSessionKey = "StateLab.VisitCount";

    [BindProperty]
    public string? UserName { get; set; }

    public string CookieUserName { get; private set; } = "нет значения";
    public string SessionUserName { get; private set; } = "нет значения";
    public int SessionVisitCount { get; private set; }

    public void OnGet()
    {
        CookieUserName = DecodeCookie(Request.Cookies[UserCookieKey]);
        SessionUserName = HttpContext.Session.GetString(UserSessionKey) ?? "нет значения";

        SessionVisitCount = (HttpContext.Session.GetInt32(VisitCountSessionKey) ?? 0) + 1;
        HttpContext.Session.SetInt32(VisitCountSessionKey, SessionVisitCount);
    }

    public IActionResult OnPostSaveServerState()
    {
        var name = NormalizeName(UserName);

        Response.Cookies.Append(
            UserCookieKey,
            WebUtility.UrlEncode(name),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Secure = Request.IsHttps,
                IsEssential = true
            });

        HttpContext.Session.SetString(UserSessionKey, name);

        var count = (HttpContext.Session.GetInt32(VisitCountSessionKey) ?? 0) + 1;
        HttpContext.Session.SetInt32(VisitCountSessionKey, count);

        return RedirectToPage();
    }

    public IActionResult OnPostClearServerState()
    {
        Response.Cookies.Delete(UserCookieKey);
        HttpContext.Session.Clear();

        return RedirectToPage();
    }

    private static string NormalizeName(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "Гость" : value.Trim();
    }

    private static string DecodeCookie(string? value)
    {
        return string.IsNullOrEmpty(value)
            ? "нет значения"
            : WebUtility.UrlDecode(value) ?? "нет значения";
    }
}
