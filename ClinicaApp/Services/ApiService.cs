using System.Text;
using System.Text.Json;
using ClinicaApp.Models;

namespace ClinicaApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://8a071a3bf7c3.ngrok-free.app";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("ngrok-skip-browser-warning", "true");
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ClinicaApp/1.0");
        }

        public async Task<ApiResponse<LoginResponse>> LoginAsync(string usuario, string password)
        {
            try
            {
                var loginRequest = new LoginRequest
                {
                    Usuario = usuario,
                    Password = password
                };

                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/auth/login", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse<LoginResponse>>(responseContent, options);
                }
                else
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Error en el servidor",
                        Status = (int)response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<ApiResponse<string>> ForgotPasswordAsync(string email)
        {
            try
            {
                var request = new ForgotPasswordRequest { Email = email };
                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{_baseUrl}/auth/olvido-password", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ApiResponse<string>>(responseContent, options);
            }
            catch (Exception ex)
            {
                return new ApiResponse<string>
                {
                    Success = false,
                    Message = $"Error de conexión: {ex.Message}",
                    Status = 500
                };
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/bypass-test");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}