# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

All commands run from the repo root. The solution is `vibe.sln`; the only project is `src/Vibe.AspNetCore/Vibe.AspNetCore.csproj`.

- Restore: `dotnet restore`
- Build: `dotnet build`
- Run (dev): `dotnet run --project src/Vibe.AspNetCore` — defaults to the `http` launch profile (`http://localhost:5270`). Use `--launch-profile https` for `https://localhost:7198`.
- Watch / hot reload: `dotnet watch --project src/Vibe.AspNetCore`
- Format: `dotnet format`
- Ad-hoc API calls: open `src/Vibe.AspNetCore/Vibe.AspNetCore.http` in an editor with REST-client support (VS / VS Code / Rider) and execute requests against the running app.

No test project exists yet. When adding one, create it under `src/` (e.g., `src/Vibe.AspNetCore.Tests`), add it to `vibe.sln` with `dotnet sln add`, and run with `dotnet test`. Filter a single test with `dotnet test --filter "FullyQualifiedName~MyTest"`.

## Architecture

- **Target framework**: `net10.0` with `Nullable` and `ImplicitUsings` both enabled (`Vibe.AspNetCore.csproj`). Code is C# with nullable reference types; rely on implicit `using`s rather than re-declaring common namespaces.
- **Web stack**: `Microsoft.NET.Sdk.Web` + attribute-routed MVC controllers (`[ApiController]`, `[Route("[controller]")]`). `Program.cs` is the minimal-hosting entry point — `AddControllers()` + `MapControllers()`, plus `AddOpenApi()` / `MapOpenApi()` which is gated to `IsDevelopment()` only.
- **OpenAPI**: provided by `Microsoft.AspNetCore.OpenApi` (built-in, not Swashbuckle). The spec is served at `/openapi/v1.json` in Development; there is no Swagger UI wired up.
- **Project layout**: only one assembly today. Controllers live in `Controllers/`, JWT options under `Authentication/`, models at the project root, namespace is `Vibe.AspNetCore` (controllers under `Vibe.AspNetCore.Controllers`, JWT options under `Vibe.AspNetCore.Authentication`). The `WeatherForecast` controller + model are scaffold leftovers — feel free to delete once real endpoints exist.
- **HTTPS**: `app.UseHttpsRedirection()` is on, so the HTTP profile will 307 to HTTPS for non-API browser hits; use the `https` launch profile or hit the API endpoints directly to avoid redirect noise during local testing.

## Authentication (JWT bearer)

- **Package**: `Microsoft.AspNetCore.Authentication.JwtBearer` is referenced in `Vibe.AspNetCore.csproj`.
- **Pipeline**: `Program.cs` calls `AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` and `AddAuthorization()`, then `app.UseAuthentication()` runs before `app.UseAuthorization()`. Token validation enforces issuer, audience, lifetime, and signing key with `ClockSkew = TimeSpan.Zero`.
- **Options**: bound from the `Jwt` configuration section into `Vibe.AspNetCore.Authentication.JwtOptions` (`Issuer`, `Audience`, `Key`, `ExpiresMinutes`). Startup throws if the section or `Key` is missing. `appsettings.json` carries non-secret defaults; `appsettings.Development.json` supplies a placeholder `Key` for local dev only — production secrets belong in user-secrets, env vars (`Jwt__Key`), or a key vault, never in `appsettings.json`.
- **Endpoints**: `POST /auth/login` (`AuthController`) accepts `{ "username", "password" }` and returns `{ "accessToken", "expiresAt" }`. Credentials are hard-coded (`demo` / `password`) as a placeholder — swap in a real user store before shipping. Every other controller is protected with `[Authorize]` (currently `WeatherForecastController`); leave `AuthController` anonymous so clients can mint tokens.
- **Calling protected endpoints**: send `Authorization: Bearer <token>`. Without it, expect `401 Unauthorized`.

When adding a new controller, default to `[Authorize]` and only opt out with `[AllowAnonymous]` where genuinely public.
