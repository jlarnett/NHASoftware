# NHASoftware

NHASoftware is a .NET 9 web application built with ASP.NET Core, Razor Pages, MVC controllers, and Entity Framework Core. The repository centers on a community-style site with social posting, forums, user identity, chat, friend management, and content-focused modules such as anime and game pages.

The solution is structured to support local development, automated testing, and Azure deployment.

## Solution Overview

This repository currently includes three projects in the main solution:

- **NHASoftware / NHA.Website.Software** - the primary ASP.NET Core web application
- **NHAHelpers** - shared helper utilities
- **NHA.Testing** - MSTest-based unit test project

## Tech Stack

- **.NET 9**
- **ASP.NET Core**
- **Razor Pages + MVC Controllers**
- **Entity Framework Core with SQL Server**
- **ASP.NET Core Identity**
- **Hangfire** for recurring background jobs
- **Azure App Configuration** and **Azure Key Vault** for production configuration
- **Application Insights** for telemetry in production
- **AutoMapper**
- **MSTest** and **NSubstitute** for testing

## Key Features

Based on the current codebase, the application includes support for:

- User registration, login, and account management via ASP.NET Core Identity
- Social posting APIs, including feature-flagged customized posts with image support
- Friend and friend request workflows
- Chat-related services and controllers
- Forum sections, topics, posts, and comments
- Anime and game content modules
- Search endpoints
- Sponsor/ad rotation services
- Background jobs for recurring maintenance and content-loading tasks

## Repository Structure

```text
.
|-- NHASoftware/        # Main ASP.NET Core web app
|-- NHAHelpers/         # Shared helper library
|-- NHA.Testing/        # Unit tests
|-- .github/workflows/  # CI/CD workflows
`-- NHA.Software.sln    # Solution file
```

## Getting Started

### Prerequisites

Before running locally, make sure you have:

- **.NET 9 SDK**
- **SQL Server** or another SQL Server-compatible environment
- A development HTTPS certificate trusted by ASP.NET Core

### Local Configuration

The app expects configuration values that are not stored directly in the repository. At minimum, provide:

- `ConnectionStrings:DefaultConnection`

Depending on the features you use, you may also need values for email and other service integrations. In production, the app is configured to load secrets from Azure Key Vault and feature/config values from Azure App Configuration.

For local development, a good approach is to use **User Secrets** or environment variables.

### Run the App

From the repository root:

1. Restore dependencies
2. Build the solution
3. Run the web app project

Common commands:

- `dotnet restore`
- `dotnet build NHA.Software.sln`
- `dotnet run --project NHASoftware/NHA.Website.Software.csproj`

The local launch settings are configured for:

- `https://localhost:7258`
- `http://localhost:5258`

## Database Notes

The application uses Entity Framework Core with SQL Server.

- The default connection is read from `ConnectionStrings:DefaultConnection`
- Database migrations are applied automatically at startup

Make sure the target database exists and the configured account has permission to apply migrations.

## Feature Flags

Development settings currently include flags for features such as:

- Anime
- Customized posts
- Forums
- Crypto
- Sponsor ads
- Game wiki

Feature management is wired into the application and can also be backed by Azure App Configuration in production.

## Background Jobs

Hangfire is configured for recurring jobs, including tasks related to:

- Profile picture cleanup
- Anime content loading
- Game content loading
- Featured content selection

The Hangfire dashboard is exposed at:

- `/hangfire`

## Testing

The repository includes an MSTest project for unit testing.

To run tests locally:

- `dotnet test NHA.Software.sln`

## CI/CD

GitHub Actions is configured to:

- Build the solution
- Run unit tests
- Publish the app
- Deploy to **Azure App Service**
- Run post-deployment Selenium/Cucumber-based validation from an external Java test repository

The current workflow targets the `master` branch and deploys the application to the Azure Web App named **NHAIndustries**.

## Production Configuration

When running in production, the application is set up to use:

- **Azure Key Vault** for secrets
- **Azure App Configuration** for configuration and feature flags
- **Application Insights** for telemetry

That means a production environment should provide the required Azure resource configuration, connection strings, and deployment secrets.

## Contributing

If you are extending the application:

- Keep changes scoped to the relevant project
- Add or update tests where practical
- Verify the solution builds cleanly before opening a PR

## Summary

NHASoftware is a multi-project ASP.NET Core web solution aimed at delivering a feature-rich community platform backed by SQL Server, Identity, background processing, and Azure-hosted deployment workflows.
