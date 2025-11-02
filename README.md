# Diff, Ergo Sum

The goal of this project is to implement a diffing API that can be used to compare two files and return the differences between them.

See [assignment details PDF](assignment.pdf) for more information.

The project is implemented in C# using .NET 8.0 and follows the [Clean Architecture](https://www.martinfowler.com/bliki/CleanArchitecture.html) principles and [TDD](https://en.wikipedia.org/wiki/Test-driven_development).

## Project structure

```
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

.git/hooks/pre-commit

```bash
#!/bin/sh
echo "Running .NET format before commit..."
dotnet format --verify-no-changes || {
  echo "❌ Code not formatted! Run 'dotnet format' to fix."
  exit 1
}
```

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

- [ ] Add API health endpoint test
- [ ] Add API health endpoint
- [ ] Add diff service test
- [ ] Add diff service
- [ ] Add diff endpoint test
- [ ] Add diff endpoint

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

##
