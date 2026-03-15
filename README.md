# Weather API

Minimal ASP.NET Core API that fetches current weather data from Open-Meteo.

## Tech Stack

- .NET 10 (`net10.0`)
- ASP.NET Core Minimal API
- Open-Meteo public API

## Project Structure

- `back.sln`
- `WeatherAPI/` (main API project)

## Endpoint

### `GET /weather?city={city}`

Returns current weather information for a known city.

Supported city values in the current implementation:

- `são paulo`
- `rio de janeiro`
- `curitiba`

Example response:

```json
{
  "city": "são paulo",
  "temperature": 24.3,
  "windSpeed": 8.7,
  "weatherCode": 3
}
```

## Prerequisites

- .NET SDK 10.0 (preview or stable, depending on your local setup)

Check your SDK:

```powershell
dotnet --version
```

## Run Locally

From the workspace root:

```powershell
dotnet restore
cd WeatherAPI
dotnet run
```

The API will start using the URL configured by your local launch settings.

## Quick Test

Using browser or HTTP client:

```text
GET http://localhost:<port>/weather?city=são%20paulo
```

Or with PowerShell:

```powershell
Invoke-RestMethod "http://localhost:<port>/weather?city=são%20paulo"
```

## Notes

- CORS is configured with `AllowAnyOrigin`, `AllowAnyHeader`, and `AllowAnyMethod`.
- Unknown cities currently default to latitude/longitude `0,0`.
