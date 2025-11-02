# Diff, Ergo Sum

The goal of this project is to implement a diffing API that compares two files and returns their differences.

See assignment details for more information: [PDF](assignment.pdf)

The project is implemented in C# using .NET 8.0 and follows:

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Test-Driven Development (TDD)](https://en.wikipedia.org/wiki/Test-driven_development)

## Project Structure

```bash
DiffErgoSum.Tests/
├── ApiHealthTest.cs             # sanity / environment test
├── DiffServiceTests.cs          # unit tests for core logic
└── IntegrationTests/            #
    └── DiffEndpointsTests.cs    # full flow

DiffErgoSum
│ # LAYER             # DEPENDS ON            # NOTES
├── Domain            # /                     # Core business rules; pure logic.
├── Application       # Domain                # Uses domain models and services.
├── Controllers       # Application           # Web entry points that coordinate via Application layer.
│   └── Models        # /                     # DTOs used for HTTP boundaries.
└── Infrastructure    # Application,Domain    # Implements persistence logic and other external systems.
```

## Pre-commit hook

When you first clone the repository, the pre-commit hook is **not enabled by default**.
See [.git-hooks/pre-commit](.git-hooks/pre-commit) for installation instructions.

### What the hook does

- Optionally auto-formats staged `*.cs` files and re-stages them (if the `<AUTO-FORMAT>` block is uncommented)
- Verifies that all staged `*.cs` files are properly formatted according to `.editorconfig`
- Aborts the commit if formatting issues are found

## TODO

### Setup

- [x] Initialize .NET 8.0 solution and projects (API + Tests)
- [x] Initialize Git repository
- [x] Sync with GitHub
- [x] Add .gitignore
- [x] Add global.json
- [x] Document architecture and dependencies in README
- [x] Configure test runner (xUnit)
- [x] Add .editorconfig
- [x] Add pre-commit hook

### Implementation

- [x] Add `ApiHealthTest` (`/api/health` sanity check)
- [x] Implement API health endpoint
- [x] Add `DiffServiceTests` (unit tests for diffing logic)
- [x] Implement diff service
- [ ] Add `DiffEndpointsTests` (integration-level)
- [ ] Implement diff endpoints

### Technical Debt / Future Refactor

Temporary test hooks (e.g., ResetRepository) exist to support isolated TDD runs.
These must not exist in production builds. Once dependency injection (DI) is introduced,
the InMemoryDiffRepository should be replaced by an injected instance (e.g., AddSingleton)
and test environments should get their own scoped or transient repository.

- [x] Temporary static repository for in-memory storage
- [x] Temporary ResetRepository() used in tests for state isolation
- [ ] Refactor to use proper DI-based repository lifetime management
- [ ] Register InMemoryDiffRepository via DI in Program.cs (AddSingleton or AddScoped)
- [ ] Remove static instance and ResetRepository() helper before production
- [ ] Replace manual repository instantiation with constructor injection in DiffController

## Initial setup done

This is only for Descartes' reference, and would usually not be a part of the repository.

```bash
➜  ~ cd dev/github.com/mathmul/
➜  mathmul dotnet --version
8.0.414
➜  mathmul mkdir diff-ergo-sum
➜  mathmul cd diff-ergo-sum
➜  diff-ergo-sum git init
Initialized empty Git repository in /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/.git/
➜  diff-ergo-sum git:(main) dotnet new webapi -n DiffErgoSum

Welcome to .NET 8.0!
---------------------
SDK Version: 8.0.414

Telemetry
---------
The .NET tools collect usage data in order to help us improve your experience. It is collected by Microsoft and shared with the community. You can opt-out of telemetry by setting the DOTNET_CLI_TELEMETRY_OPTOUT environment variable to '1' or 'true' using your favorite shell.

Read more about .NET CLI Tools telemetry: https://aka.ms/dotnet-cli-telemetry

----------------
Installed an ASP.NET Core HTTPS development certificate.
To trust the certificate, run 'dotnet dev-certs https --trust'
Learn about HTTPS: https://aka.ms/dotnet-https

----------------
Write your first app: https://aka.ms/dotnet-hello-world
Find out what's new: https://aka.ms/dotnet-whats-new
Explore documentation: https://aka.ms/dotnet-docs
Report issues and find source on GitHub: https://github.com/dotnet/core
Use 'dotnet --help' to see available commands or visit: https://aka.ms/dotnet-cli
--------------------------------------------------------------------------------------
An issue was encountered verifying workloads. For more information, run "dotnet workload update".
The template "ASP.NET Core Web API" was created successfully.

Processing post-creation actions...
Restoring /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/DiffErgoSum/DiffErgoSum.csproj:
  Determining projects to restore...
  Restored /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/DiffErgoSum/DiffErgoSum.csproj (in 4.26 sec).
Restore succeeded.


➜  diff-ergo-sum git:(main) ✗ dotnet new xunit -n DiffErgoSum.Tests
The template "xUnit Test Project" was created successfully.

Processing post-creation actions...
Restoring /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/DiffErgoSum.Tests/DiffErgoSum.Tests.csproj:
  Determining projects to restore...
  Restored /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/DiffErgoSum.Tests/DiffErgoSum.Tests.csproj (in 8.43 sec).
Restore succeeded.


➜  diff-ergo-sum git:(main) ✗ dotnet add DiffErgoSum.Tests reference DiffErgoSum
Reference `..\DiffErgoSum\DiffErgoSum.csproj` added to the project.
➜  diff-ergo-sum git:(main) ✗ dotnet new sln -n DiffErgoSum
The template "Solution File" was created successfully.

➜  diff-ergo-sum git:(main) ✗ dotnet sln add DiffErgoSum DiffErgoSum.Tests
Project `DiffErgoSum/DiffErgoSum.csproj` added to the solution.
Project `DiffErgoSum.Tests/DiffErgoSum.Tests.csproj` added to the solution.
➜  diff-ergo-sum git:(main) ✗ tree -L 2
.
├── DiffErgoSum
│   ├── appsettings.Development.json
│   ├── appsettings.json
│   ├── DiffErgoSum.csproj
│   ├── DiffErgoSum.http
│   ├── obj
│   ├── Program.cs
│   └── Properties
├── DiffErgoSum.sln
└── DiffErgoSum.Tests
    ├── DiffErgoSum.Tests.csproj
    ├── obj
    └── UnitTest1.cs

6 directories, 8 files
➜  diff-ergo-sum git:(main) ✗ dotnet new gitignore
The template "dotnet gitignore file" was created successfully.

➜  diff-ergo-sum git:(main) ✗ cat > global.json <<'EOF'
{
  "sdk": {
    "version": "8.0.414",
    "rollForward": "latestFeature"
  }
}
EOF
➜  diff-ergo-sum git:(main) ✗ dotnet run --project DiffErgoSum
Building...
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5093
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: /Users/s3c/dev/github.com/mathmul/diff-ergo-sum/DiffErgoSum
```
