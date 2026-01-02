# ItDepends

ItDepends is the first SBaaS (Smart Boolean as a Service) solution for evaluating dynamic freeform expressions using AI into boolean responses.

## Projects
- ItDepends.API: Minimal API providing boolean utilities, 8-ball responses, and SmartBoolean (OpenAI-backed) answers. Uses FluentValidation, Serilog, and Aspire OpenAI client wiring.
- ItDepends.AppHost: Aspire host defining the OpenAI resource and wiring the API project.
- ItDepends.ServiceDefaults: Shared defaults for telemetry, health checks, and service discovery.

## Endpoints (API)
- GET /api/boolean/random|true|false
- GET /api/8ball
- POST /api/smart/boolean { prompt }

## Prerequisites
- .NET 8/10 SDK
- OpenAI API key (for SmartBoolean)

## Configure
- Set OpenAI key via Aspire resource (preferred) or env: `OPENAI_API_KEY`.
- Logging via Serilog configured in `appsettings.json`.

## Run
- Local: `dotnet run --project ItDepends.API/ItDepends.API.csproj`
- Aspire host: `dotnet run --project ItDepends.AppHost/ItDepends.AppHost.csproj`

## Tests
- (None yet) Add API/integration tests as needed.
