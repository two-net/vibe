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
- **Project layout**: only one assembly today. Controllers live in `Controllers/`, models at the project root, namespace is `Vibe.AspNetCore` (controllers under `Vibe.AspNetCore.Controllers`). The `WeatherForecast` controller + model are scaffold leftovers — feel free to delete once real endpoints exist.
- **HTTPS**: `app.UseHttpsRedirection()` is on, so the HTTP profile will 307 to HTTPS for non-API browser hits; use the `https` launch profile or hit the API endpoints directly to avoid redirect noise during local testing.
