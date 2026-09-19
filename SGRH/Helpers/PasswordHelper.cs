using BCrypt.Net;

namespace SGRH.Helpers
{
    public static class PasswordHelper
    {
        public static string Hash(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha);
        }

        public static bool Verificar(string senha, string hashArmazenado)
        {
            if (string.IsNullOrEmpty(hashArmazenado))
                return false;

            if (!EhHashBcrypt(hashArmazenado))
                return senha == hashArmazenado;

            try
            {
                return BCrypt.Net.BCrypt.Verify(senha, hashArmazenado);
            }
            catch
            {
                return false;
            }
        }

        public static bool EhHashBcrypt(string hashArmazenado)
        {
            return !string.IsNullOrEmpty(hashArmazenado)
                && (hashArmazenado.StartsWith("$2a$")
                    || hashArmazenado.StartsWith("$2b$")
                    || hashArmazenado.StartsWith("$2y$"));
        }
    }
}