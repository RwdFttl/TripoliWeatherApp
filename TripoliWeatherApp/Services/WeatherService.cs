using System.Net.Http.Json;
using System.Text.Json.Serialization;


namespace TripoliWeatherApp.Services;

public class WeatherService
{
    private readonly HttpClient _http;
    private readonly string cityName = "Tripoli";

    public WeatherService(HttpClient http)
    {
        _http = http;
    }

    public async Task<CurrentWeather?> GetGeoWeatherAsync()
    {
        GeoResponse? geoResponse = await _http.GetFromJsonAsync<GeoResponse>($"https://geocoding-api.open-meteo.com/v1/search?name={cityName}&count=10");
        GeoLocation? geoLocation = geoResponse?.results?.FirstOrDefault(city => city.countryCode == "LB");
        if (geoLocation is null) return null;
        string weatherUrl = $"https://api.open-meteo.com/v1/forecast?latitude={geoLocation.latitude}&longitude={geoLocation.longitude}&current=temperature_2m,relative_humidity_2m,weather_code,wind_speed_10m&timezone=auto";
        WeatherResponse? weatherResponse = await _http.GetFromJsonAsync<WeatherResponse>(weatherUrl);
        return weatherResponse?.current;
    }
}
public class GeoResponse
{
    [JsonPropertyName("results")]
    public List<GeoLocation>? results { get; set; }

}
public class GeoLocation
{
    [JsonPropertyName("country_code")]
    public string? countryCode { get; set; }

    [JsonPropertyName("latitude")]
    public double latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double longitude { get; set; }
}

public class WeatherResponse
{
    [JsonPropertyName("current")]
    public CurrentWeather? current { get; set; }
}

public class CurrentWeather
{
    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public int Humidity { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }

    public string WeatherCodeDescription => WeatherCode switch
    {
        0 => "Sunny",
        1 => "Mostly sunny",
        2 => "Partly cloudy",
        3 => "Cloudy",
        45 or 48 => "Fog",
        51 or 53 or 55 => "Drizzle",
        56 or 57 => "Freezing drizzle",
        61 or 63 or 65 => "Rain",
        66 or 67 => "Freezing rain",
        71 or 73 or 75 => "Snow",
        77 => "Snow flurries",
        80 or 81 or 82 => "Scattered showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorms",
        96 or 97 or 99 => "Thunderstorms with hail",
        _ => "Unknown"
    };

}
