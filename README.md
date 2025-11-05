# Diff, Ergo Sum

The goal of this project is to implement a diffing API that compares two files and returns their differences.

See assignment details for more information: [PDF](assignment.pdf)

The project is implemented in C# using .NET 8.0 and follows:

- [Clean Architecture](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [Test-Driven Development (TDD)](https://en.wikipedia.org/wiki/Test-driven_development)

## Project Structure

```bash
DiffErgoSum.Tests/
├── ApiHealthTest.cs                        # sanity / environment test
├── DiffServiceTests.cs                     # unit tests for core logic
└── IntegrationTests/                       #
    ├── DiffEndpointsTests.cs               # full flow
    └── DiffRepositoryIntegrationTest.cs    # DB integration (postgres)

DiffErgoSum
│ # LAYER             # DEPENDS ON     # NOTES
├── Domain            # /              # Core business logic and rules; pure, framework-independent.
│   └── Validators    # /              # Domain-agnostic validation helpers or data checks.
│
├── Application       # Domain         # Uses domain models and services, and is injected into controllers.
│
├── Controllers       # Application    # API entry points; translate HTTP requests/responses to/from the Application layer.
│   ├── Exceptions    # /              # Custom HTTP-friendly exception types.
│   ├── Filters       # /              # ASP.NET filters for request validation and error handling.
│   └── Models        # /              # DTOs used for HTTP boundaries.
│
├── Middleware        # Controllers    # Cross-cutting concerns (e.g. error handling) that wrap HTTP requests.
│
└── Infrastructure    # Domain         # Implements persistence logic and other external systems.
    └── Entities      # /              # Database or storage entities.
```

## Pre-commit hook

When you first clone the repository, the pre-commit hook is **not enabled by default**.
See [.git-hooks/pre-commit](.git-hooks/pre-commit) for installation instructions.

### What the hook does

- Optionally auto-formats staged `*.cs` files and re-stages them (if the `<AUTO-FORMAT>` block is uncommented)
- Verifies that all staged `*.cs` files are properly formatted according to `.editorconfig`
- Aborts the commit if formatting issues are found

## TODO

<details><summary>Setup</summary>

- [x] Initialize .NET 8.0 solution and projects (API + Tests)
- [x] Initialize Git repository
- [x] Sync with GitHub
- [x] Add .gitignore
- [x] Add global.json
- [x] Document architecture and dependencies in README
- [x] Configure test runner (xUnit)
- [x] Add .editorconfig
- [x] Add pre-commit hook

</details>

<details><summary>Implementation (TDD)</summary>

- [x] Add `ApiHealthTest` (`/api/health` sanity check)
- [x] Implement API health endpoint
- [x] Add `DiffServiceTests` (unit tests for diffing logic)
- [x] Implement diff service (Equals, SizeDoNotMatch, ContentDoNotMatch)
- [x] Add `DiffEndpointsTests` (integration-level, /api/v1/diff/{id}/left|right|get)
- [x] Implement diff endpoints using in-memory storage
- [x] Add input/data validation tests (null / missing `data` / invalid base64)
- [x] Implement explicit validation + 4xx responses (match PDF assignment)
- [x] Add tests to cover **all** cases from the PDF sample (1–10)
- [x] Add tests for “right exists, left missing” and vice versa (should 404)
- [x] Add tests for malformed JSON bodies

</details>

<details><summary>Technical Debt / Future Refactor</summary>

Temporary test hooks (e.g. `ResetRepository`) exist to support isolated TDD runs.
These must **not** exist in production builds. Once DI is introduced, the in-memory
repository should be registered via DI and test envs should get their own instance.

- [x] Temporary static repository for in-memory storage
- [x] <del>Temporary `ResetRepository()` used in tests for state isolation</del>
- [x] Refactor to use proper DI-based repository lifetime management
- [x] Register `InMemoryDiffRepository` via DI in `Program.cs` (`AddSingleton` or `AddScoped`)
- [x] Remove static instance and `ResetRepository()` helper before production
- [x] Replace manual repo instantiation in `DiffController` with constructor injection
- [ ] Consider splitting layers into separate projects if "assignment" grows
- [ ] Replace or extend to InMemoryDiffRepository:
      - **PostgreSQL** — for long-term persistence and auditability (with EF Core or Dapper + migrations)
      - **Redis** — for high-volume, repeated requests or temporary caching (hash per `diff:{id}`)
      - **Filesystem / Object Storage** — for large binary payloads or archival
      - **SQLite** — for lightweight, embedded setups or demos
- [x] Introduce an `IDiffRepository` interface to enforce dependency inversion — controllers and services depend only on the abstraction, not the implementation
- [ ] Provide schema or configuration for the chosen backend (SQL migrations or Redis setup)
- [ ] Add integration tests for the persistent repository implementation
- [ ] Make repository implementation configurable via environment variables (e.g., `STORAGE=postgres|redis|memory`)

</details>

<details><summary>Error Handling / API UX</summary>

- [x] Explicit error responses (HTTP status codes)
- [x] Standardize error shape (e.g. `{ "error": "InvalidBase64", "message": "..." }`)
- [x] Catch expected exceptions (eg. `FormatException` from `Convert.FromBase64String`) and return 422 with message
- [x] Add tests for invalid base64 on GET (stored bad data)
- [x] Document error responses in README and/or OpenAPI

</details>

<details><summary>Testing Strategy</summary>

- [x] Cover every example from the assignment PDF (10-step sample)
- [ ] Add regression test for "same size but multiple contiguous diffs"
- [ ] Consider property-based testing for diff logic
  - e.g. generate two byte arrays of same length → diffs must be non-overlapping and ordered
  - e.g. when arrays are identical → always `Equals`
  - e.g. when lengths differ → always `SizeDoNotMatch`
- [ ] Consider base64 roundtrip tests (store → decode → diff)

</details>

<details><summary>Tooling / DevEx</summary>

- [x] Dockerize the project (multi-stage build, `dotnet publish`)
- [x] Add "Getting Started" section to README (local run + docker run)
- [ ] Add `.github/workflows/dotnet.yml` to run `dotnet build` + `dotnet test` on PRs
- [x] Add `dotnet format` to CI to enforce style (ready in Makefile, not used in CI/CD yet)

</details>

<details><summary>Code Quality / Static Analysis</summary>

- [x] Enable built-in Roslyn analyzers via `<AnalysisLevel>latest</AnalysisLevel>` and `<EnforceCodeStyleInBuild>true</EnforceCodeStyleInBuild>`
- [x] Add NuGet packages for enhanced static analysis:
      - `Microsoft.CodeAnalysis.NetAnalyzers`
      - `Microsoft.CodeAnalysis.Analyzers`
      - `Microsoft.VisualStudio.Threading.Analyzers` (optional, async/threading)
      - `SonarAnalyzer.CSharp` (for security and maintainability)
      - `StyleCop.Analyzers`
- [x] Keep default rule severities for consistency across projects (avoid custom .ruleset initially)
- [x] Verify that no `.ruleset` or `.editorconfig` overrides exist during code review
- [ ] Integrate analyzer warnings into CI build (treat as errors only after stabilizing)
- [ ] Revisit rules customization once the project matures (post-MVP)

</details>

<details><summary>API Documentation (Swagger)</summary>

- [x] Add OpenAPI for dev environment (`.AddSwaggerGen`)
- [x] Restrict Swagger UI to development environment
- [x] Add Swagger metadata (`.SwaggerDoc`: title, version, description, contact)
- [x] Enable XML comments for automatic endpoint documentation (csproj)
- [x] Document all endpoints with XML comments
- [x] Move Swagger UI to /docs for clarity
- [ ] Harden Swagger for production (protect route / authorization)
- [ ] Optionally publish Swagger JSON to CI/CD artifacts for API client generation

</details>

<details><summary>Nice to Have</summary>

- [x] Add versioning note (`/api/v1/...`)
- [ ] Add example `curl` requests to README
- [ ] Add notes on assumptions (in-memory only, no persistence, not thread-safe for prod)

</details>

## Getting Started

### Prerequisites

Make sure you have the following installed:

- .NET 8 SDK – for local development and running tests
- Docker + Docker Compose – for containerized environments
- GNU Make – to run project commands

### Usage

Run commands with

```bash
make <command>
```

To list all commands, just run

```bash
make
```

### Local Development

Runs the API directly on your machine (not dockerized).
If you choose the Postgres backend, a Postgres container will be started automatically.

| Command | Description |
| - | - |
| make serve-inmemory | Run the API with an in-memory database |
| make test-inmemory | Run tests using an in-memory database |
| make serve-inmemory | Run the API with SQLite |
| make test-inmemory | Run tests using SQLite |
| make serve-inmemory | Run the API with Dockerized Postgres |
| make test-inmemory | Run tests using Dockerized Postgres |

### Docker Workflow

Runs the API and dependencies fully inside Docker containers.
Separate containers are used for development and testing.

| Command | Description |
| - | - |
| make up | Start the full Docker stack |
| make up-build | Build and start all services |
| make build | Rebuild the API image (no cache) |
| make down | Stop containers |
| make logs | Tail container logs |
| make bash | Open a shell in the API container |
| make list | List active containers |

### Maintenance & Utilities

Utility commands for rebuilding, formatting, and database resets.
All run within the Dockerized environment unless otherwise noted.

| Command | Description |
| - | - |
| make test | Run the full test suite inside the dev container |
| make lint | Check code formatting |
| make lint-fix | Auto-fix code formatting |
| make refresh | Rebuild and restart stack (no cache) |
| make refresh-full | Rebuild everything and clear volumes |
| make reset-db | Reset Postgres database volume |

## Initial setup done

<details>
<summary>This is only for Descartes' reference, and would usually not be a part of the repository.</summary>

<pre>
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
</pre>
</details>
