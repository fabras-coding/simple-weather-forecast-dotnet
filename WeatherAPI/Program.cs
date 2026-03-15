using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});



var app = builder.Build();
app.UseCors();

app.MapGet("/weather", async (string city, IHttpClientFactory httpFactory) =>
{
   var coordinates = city.ToLower() switch
   {
       "são paulo" => (Lat: -23.55, Lon: -46.63),
       "rio de janeiro" => (Lat: -22.91, Lon: -43.17),
       "curitiba" => (Lat: -25.42, Lon: -49.27),
       _ => (Lat: 0.0, Lon: 0.0)
   };

    var url =
    $"https://api.open-meteo.com/v1/forecast?latitude={coordinates.Lat.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={coordinates.Lon.ToString(System.Globalization.CultureInfo.InvariantCulture)}&current_weather=true";

    var httpClient = httpFactory.CreateClient();
    var json = await httpClient.GetStringAsync(url);

    var response = JsonSerializer.Deserialize<OpenMeteoResponse>(json, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode
    });

    if (response is null)
        return Results.NotFound($"No weather data found for {city}");
    
    return Results.Ok(new
    {
        City = city,
        Temperature = response.CurrentWeather.Temperature,
        WindSpeed = response.CurrentWeather.WindSpeed,
        WeatherCode = response.CurrentWeather.WeatherCode
    });

});


app.Run();

record OpenMeteoResponse([property: JsonPropertyName("current_weather")] CurrentWeather CurrentWeather);
record CurrentWeather(
    [property: JsonPropertyName("temperature")] double Temperature,
    [property: JsonPropertyName("windspeed")] double WindSpeed,
    [property: JsonPropertyName("weathercode")] int WeatherCode  );