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


var cities = new List<City>
{
    new City("São Paulo", -23.55, -46.63),
    new City("Rio de Janeiro", -22.91, -43.17),
    new City("Curitiba", -25.42, -49.27),
    new City("New York", 40.7128, -74.0060),
    new City("London", 51.5074, -0.1278),
    new City("Paris", 48.8566, 2.3522),
    new City("Tokyo", 35.6762, 139.6503),
    new City("Sydney", -33.8688, 151.2093),
    new City("Dubai", 25.2048, 55.2708),
    new City("Barcelona", 41.3851, 2.1734),
    new City("Rome", 41.9028, 12.4964),
    new City("Bangkok", 13.7563, 100.5018),
    new City("Singapore", 1.3521, 103.8198),
    new City("Hong Kong", 22.3193, 114.1694),
    new City("Los Angeles", 34.0522, -118.2437),
    new City("Berlin", 52.5200, 13.4050),
    new City("Amsterdam", 52.3676, 4.9041),
    new City("Venice", 45.4408, 12.3155),
    new City("Istanbul", 41.0082, 28.9784),
    new City("Dubai", 25.2048, 55.2708),
    new City("Moscow", 55.7558, 37.6173),
    new City("Toronto", 43.6532, -79.3832),
    new City("Vancouver", 49.2827, -123.1207),
    new City("Mexico City", 19.4326, -99.1332),
    new City("San Francisco", 37.7749, -122.4194),
    new City("Miami", 25.7617, -80.1918),
    new City("Dubai", 25.2048, 55.2708),
    new City("Singapore", 1.3521, 103.8198),
    new City("Tokyo", 35.6762, 139.6503)
};


var app = builder.Build();
app.UseCors();

app.MapGet("/weather", async (string city, IHttpClientFactory httpFactory) =>
{
   var cityData = cities.FirstOrDefault(c => c.Name.Equals(city, StringComparison.OrdinalIgnoreCase));
   
   if (cityData is null)
       return Results.BadRequest($"City '{city}' not found. Available cities: {string.Join(", ", cities.Select(c => c.Name))}");

    var url =
    $"https://api.open-meteo.com/v1/forecast?latitude={cityData.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&longitude={cityData.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)}&daily=temperature_2m_min,temperature_2m_max,weather_code&current_weather=true";

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
        City = cityData.Name,
        Temperature = response.CurrentWeather.Temperature,
        WindSpeed = response.CurrentWeather.WindSpeed,
        WeatherCode = response.CurrentWeather.WeatherCode,
        Daily = response.Daily.Time.Select((time, index) => new
        {
            Date = time,
            MinTemperature = response.Daily.Temperature2mMin[index],
            MaxTemperature = response.Daily.Temperature2mMax[index],
            WeatherCode = response.Daily.DailyWeatherCode[index]
        }).ToList()
    });

});


app.Run();

record City (string Name, double Latitude, double Longitude);
record OpenMeteoResponse(
    [property: JsonPropertyName("current_weather")] CurrentWeather CurrentWeather,
    [property: JsonPropertyName("daily")] Daily Daily
    );
record CurrentWeather(
    [property: JsonPropertyName("temperature")] double Temperature,
    [property: JsonPropertyName("windspeed")] double WindSpeed,
    [property: JsonPropertyName("weathercode")] int WeatherCode  );

record Daily(
    [property: JsonPropertyName("time")] List<string> Time,
    [property: JsonPropertyName("temperature_2m_min")] List<double> Temperature2mMin,
    [property: JsonPropertyName("temperature_2m_max")] List<double> Temperature2mMax,
    [property: JsonPropertyName("weather_code")] List<int> DailyWeatherCode
);