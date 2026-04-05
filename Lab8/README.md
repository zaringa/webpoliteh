# Lab8 - Static Files in ASP.NET Core

## Goal
Learn how to add, organize and serve static files in ASP.NET Core.

## Implemented tasks
1. Added static files of different types:
- HTML: `/wwwroot/index.html`, `/wwwroot/pages/info.html`
- CSS: `/wwwroot/assets/css/site.css`
- JavaScript: `/wwwroot/assets/js/main.js`
- Image: `/wwwroot/assets/images/gallery/photo.jpg`
2. Organized resources using nested folders (`assets/images/gallery`, `pages`).
3. Configured static file serving in `Program.cs` with:
- `app.UseDefaultFiles();`
- `app.UseStaticFiles();`
4. Ready to verify via browser.

## Run
```bash
dotnet run --no-launch-profile --project Lab8 --urls http://localhost:5228
```

## Check in browser
- `http://localhost:5228/`
- `http://localhost:5228/pages/info.html`
- `http://localhost:5228/assets/css/site.css`
- `http://localhost:5228/assets/js/main.js`
- `http://localhost:5228/assets/images/gallery/photo.jpg`
