using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicaApp.Models
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }

    public class LoginResponse
    {
        public User Usuario { get; set; }
        public List<Menu> Menus { get; set; }
        public string SessionId { get; set; }
    }

    public class LoginRequest
    {
        public string Usuario { get; set; }
        public string Password { get; set; }
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; }
    }
}