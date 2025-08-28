using System.Text.Json.Serialization;

namespace ClinicaApp.Models
{
    public class ApiResponse<T>
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public T Data { get; set; }
    }

    public class LoginResponse
    {
        [JsonPropertyName("usuario")]
        public User Usuario { get; set; }

        [JsonPropertyName("menus")]
        public List<Menu> Menus { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }

    public class LoginRequest
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class ForgotPasswordResponse
    {
        [JsonPropertyName("password_temporal")]
        public string PasswordTemporal { get; set; }

        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("nota")]
        public string Nota { get; set; }
    }
}