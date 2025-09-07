// Helpers/UserSession.cs
using ClinicaApp.Models;

namespace ClinicaApp.Helpers
{
    public static class UserSession
    {
        public static LoginResponse CurrentUser { get; private set; }
        public static bool IsLoggedIn => CurrentUser != null;

        public static void SetUser(LoginResponse user)
        {
            CurrentUser = user;
        }

        public static void ClearUser()
        {
            CurrentUser = null;
        }

        // Helper específico para obtener ID del paciente
        public static int GetPacienteId()
        {
            if (CurrentUser?.Data?.Usuario?.IdRol == 4) // Rol paciente
            {
                return CurrentUser.Data.Usuario.IdUsuario;
            }
            return 0;
        }

        public static bool IsPaciente()
        {
            return CurrentUser?.Data?.Usuario?.IdRol == 4;
        }
    }
}