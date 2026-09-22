using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace AdminWPF
{
    public class ApiService
    {
        private static readonly HttpClient _http;

        static ApiService()
        {
            _http = new HttpClient { BaseAddress = new Uri("https://sutkivmv.ru") };
        }

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

            TokenStorage.Save(result!.Token);
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

            return (true, result, null);
        }
    }
}
