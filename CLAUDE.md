# CLAUDE.md

@AGENTS.md

## Skill routing

### Plugin skills -- when to load

**Always load the relevant skill before writing code.** These provide deep
pattern knowledge for C# and .NET conventions used in this project.

| Task | Load these skills |
|------|-------------------|
| New C# code | `dotnet-skills:csharp-coding-standards` |
| New request/type record | `dotnet-skills:serialization` |
| Public API changes | `dotnet-skills:csharp-api-design` |
| Type design (seal, readonly) | `dotnet-skills:csharp-type-design-performance` |
| Nullable annotations | `dotnet-skills:csharp-nullable-reference-types` |
| R3 Observable streams | `dotnet-skills:r3-reactive-extensions` |
| NuGet packages | `dotnet-skills:package-management` |
| Project/csproj changes | `dotnet-skills:project-structure` |
| Aspire AppHost | `dotnet-skills:aspire-configuration` |
| Test coverage analysis | `dotnet-skills:crap-analysis` |
| Snapshot testing (Verify) | `dotnet-skills:snapshot-testing` |
| Docs site (DocFX) | `dotnet-skills:docfx-specialist` |
| Concurrency patterns | `dotnet-skills:csharp-concurrency-patterns` |
