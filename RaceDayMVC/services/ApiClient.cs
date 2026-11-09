using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDayMVC.Services;

public class ApiClient
{
    private readonly HttpClient _http;
    private readonly IHttpContextAccessor _context;
    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiClient(HttpClient http, IHttpContextAccessor context)
    {
        _http = http;
        _context = context;
        // ⚠️ IMPORTANT: Change 7102 if your API runs on a different port!
        _http.BaseAddress = new Uri("https://localhost:7102/api/");
    }

    // Attach the API's session cookie (stored in MVC's session) to every call
    private void AttachCookie()
    {
        var apiCookie = _context.HttpContext?.Session.GetString("ApiCookie");
        if (!string.IsNullOrEmpty(apiCookie))
        {
            _http.DefaultRequestHeaders.Clear();
            _http.DefaultRequestHeaders.Add("Cookie", apiCookie);
        }
    }

    // Save the API's cookie back into MVC's session after login
    public void SaveCookieFromResponse(HttpResponseMessage response)
    {
        if (response.Headers.TryGetValues("Set-Cookie", out var cookies))
        {
            var cookie = string.Join("; ", cookies.Select(c => c.Split(';')[0]));
            _context.HttpContext?.Session.SetString("ApiCookie", cookie);
        }
    }

    public async Task<HttpResponseMessage> GetAsync(string route)
    {
        AttachCookie();
        return await _http.GetAsync(route);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string route, T body)
    {
        AttachCookie();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PostAsync(route, content);
    }

    public async Task<HttpResponseMessage> PutAsync<T>(string route, T body)
    {
        AttachCookie();
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        return await _http.PutAsync(route, content);
    }

    public async Task<HttpResponseMessage> DeleteAsync(string route)
    {
        AttachCookie();
        return await _http.DeleteAsync(route);
    }

    public async Task<T?> ReadAsync<T>(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode) return default;
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOpts);
    }
    public async Task<HttpResponseMessage> PostFileAsync(string route, IFormFile file)
    {
        AttachCookie();
        using var content = new MultipartFormDataContent();
        using var stream = file.OpenReadStream();
        var fileContent = new StreamContent(stream);
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
        content.Add(fileContent, "file", file.FileName);

        return await _http.PostAsync(route, content);
    }

}