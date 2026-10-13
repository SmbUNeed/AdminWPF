using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace AdminWPF.Services;

public static class ApiService
{
    static readonly HttpClient _http = new() { BaseAddress = new Uri("https://sutkivmv.ru") };

    public static void SetToken(string token) =>
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public static async Task<(bool Success, AuthResponseDto? Data, string? Error)> LoginAsync(string email, string password)
    {
        var dto = new LoginDto { Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("api/auth/login", dto);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            return (false, null, error?.GetValueOrDefault("message"));
        }

        var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
        return (true, result, null);
    }

    public static Task<List<T>?> GetListAsync<T>(string url) => _http.GetFromJsonAsync<List<T>>(url);
    public static Task<T?> GetAsync<T>(string url) => _http.GetFromJsonAsync<T>(url);
    public static Task<HttpResponseMessage> PostAsync<T>(string url, T dto) => _http.PostAsJsonAsync(url, dto);
    public static Task<HttpResponseMessage> PutAsync<T>(string url, T dto) => _http.PutAsJsonAsync(url, dto);
    public static Task<HttpResponseMessage> DeleteAsync(string url) => _http.DeleteAsync(url);
}