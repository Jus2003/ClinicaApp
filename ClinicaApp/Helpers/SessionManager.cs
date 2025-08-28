using ClinicaApp.Models;

namespace ClinicaApp.Helpers
{
    public static class SessionManager
    {
        public static User CurrentUser { get; set; }
        public static List<Menu> UserMenus { get; set; } = new List<Menu>();
        public static string SessionId { get; set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static void SetUserSession(LoginResponse loginResponse)
        {
            CurrentUser = loginResponse.Usuario;
            UserMenus = loginResponse.Menus ?? new List<Menu>();
            SessionId = loginResponse.SessionId;
        }

        public static void ClearSession()
        {
            CurrentUser = null;
            UserMenus = new List<Menu>();
            SessionId = null;
        }
    }
}