using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Windows;

namespace AdminWPF.Services;

public static class ApiService
{
    static readonly HttpClient _http = new() { BaseAddress = new Uri("https://sutkivmv.ru"), Timeout = TimeSpan.FromSeconds(20) };

    public static string? CurrentEmail { get; private set; }

    public static void SetToken(string token) =>
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public static void ClearToken()
    {
        _http.DefaultRequestHeaders.Authorization = null;
        CurrentEmail = null;
    }

    static async Task<T> Safe<T>(Func<Task<T>> action, T fallback)
    {
        try
        {
            return await action();
        }
        catch (HttpRequestException ex)
        {
            Show(ex.StatusCode is { } code ? $"Ошибка сервера: {(int)code}" : "Нет соединения с сервером");
        }
        catch (TaskCanceledException)
        {
            Show("Превышено время ожидания ответа");
        }
        catch (JsonException)
        {
            Show("Некорректный ответ сервера");
        }
        catch (Exception ex)
        {
            Show(ex.Message);
        }
        return fallback;
    }

    static void Show(string message) =>
        Application.Current.Dispatcher.Invoke(() => MessageBox.Show(message, "Ошибка"));

    static HttpResponseMessage Failed() => new(HttpStatusCode.ServiceUnavailable);

    public static async Task<(bool Success, AuthResponseDto? Data, string? Error)> LoginAsync(string email, string password)
    {
        try
        {
            var dto = new LoginDto { Email = email, Password = password };
            var response = await _http.PostAsJsonAsync("api/auth/login", dto);

            if (!response.IsSuccessStatusCode)
            {
                string? message = null;
                try
                {
                    var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    message = error?.GetValueOrDefault("message");
                }
                catch (JsonException) { }
                return (false, null, message ?? $"Ошибка {(int)response.StatusCode}");
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result?.Token == null) return (false, null, "Отсутствует токен");

            SetToken(result.Token);
            CurrentEmail = email.Trim().ToLowerInvariant();
            return (true, result, null);
        }
        catch (HttpRequestException)
        {
            return (false, null, "Нет соединения с сервером");
        }
        catch (TaskCanceledException)
        {
            return (false, null, "Превышено время ожидания ответа");
        }
        catch (Exception ex)
        {
            return (false, null, ex.Message);
        }
    }

    public static async Task<(bool Success, string? Error)> RequestPasswordChangeAsync()
    {
        if (CurrentEmail == null) return (false, "Email неизвестен, войди заново");
        try
        {
            var response = await _http.PostAsJsonAsync("api/auth/change-password-email", CurrentEmail);
            return response.IsSuccessStatusCode ? (true, null) : (false, $"Ошибка {(int)response.StatusCode}");
        }
        catch (HttpRequestException)
        {
            return (false, "Нет соединения с сервером");
        }
        catch (Exception ex)
        {
            return (false, ex.Message);
        }
    }

    public static Task<List<T>?> GetListAsync<T>(string url) =>
        Safe<List<T>?>(() => _http.GetFromJsonAsync<List<T>>(url), null);

    public static Task<T?> GetAsync<T>(string url) =>
        Safe<T?>(() => _http.GetFromJsonAsync<T>(url), default);

    public static Task<HttpResponseMessage> PostAsync<T>(string url, T dto) =>
        Safe(() => _http.PostAsJsonAsync(url, dto), Failed());

    public static Task<HttpResponseMessage> PutAsync<T>(string url, T dto) =>
        Safe(() => _http.PutAsJsonAsync(url, dto), Failed());

    public static Task<HttpResponseMessage> DeleteAsync(string url) =>
        Safe(() => _http.DeleteAsync(url), Failed());
}