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
            // MATAR TODO ANTES DE SETEAR
            NukeEverything();

            CurrentUser = loginResponse.Usuario;
            UserMenus = loginResponse.Menus ?? new List<Menu>();
            SessionId = loginResponse.SessionId;

            System.Diagnostics.Debug.WriteLine($"✅ SESIÓN ESTABLECIDA: {CurrentUser?.Nombre}");
        }

        public static void ClearSession()
        {
            System.Diagnostics.Debug.WriteLine("🔥 NUKEAR SESIÓN COMPLETAMENTE");
            NukeEverything();
        }

        private static void NukeEverything()
        {
            try
            {
                // 1. MATAR memoria
                CurrentUser = null;
                UserMenus = new List<Menu>();
                SessionId = null;

                // 2. MATAR Preferences
                Preferences.Clear();

                // 3. MATAR claves específicas por si acaso
                try { Preferences.Remove("user_data"); } catch { }
                try { Preferences.Remove("user_menus"); } catch { }
                try { Preferences.Remove("session_id"); } catch { }
                try { Preferences.Remove("remember_user"); } catch { }
                try { Preferences.Remove("auto_login"); } catch { }

                // 4. FORZAR garbage collection
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                System.Diagnostics.Debug.WriteLine("💀 TODO NUKADO");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error nukeando: {ex.Message}");
            }
        }

        public static bool HasValidSession()
        {
            return CurrentUser != null;
        }
    }
}