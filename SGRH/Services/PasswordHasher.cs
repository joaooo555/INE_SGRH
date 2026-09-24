using System.Security.Cryptography;

namespace SGRH.Services
{
    /// <summary>
    /// RT07 — Passwords Seguras: palavras-passe armazenadas utilizando funções de hash seguras (PBKDF2-SHA256).
    /// Formato armazenado: {iteracoes}.{salt_base64}.{hash_base64}
    /// </summary>
    public interface IPasswordHasher
    {
        string Hash(string senha);
        bool Verificar(string senha, string hashArmazenado);
    }

    public class PasswordHasher : IPasswordHasher
    {
        private const int Iteracoes = 100_000;
        private const int TamanhoHash = 32;

        public string Hash(string senha)
        {
            var salt = RandomNumberGenerator.GetBytes(32);
            var hash = Rfc2898DeriveBytes.Pbkdf2(senha, salt, Iteracoes, HashAlgorithmName.SHA256, TamanhoHash);
            return $"{Iteracoes}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
        }

        public bool Verificar(string senha, string hashArmazenado)
        {
            if (string.IsNullOrWhiteSpace(hashArmazenado))
                return false;

            // Hashes BCrypt (formato novo) são delegadas ao PasswordHelper.
            if (SGRH.Helpers.PasswordHelper.EhHashBcrypt(hashArmazenado))
                return SGRH.Helpers.PasswordHelper.Verificar(senha, hashArmazenado);

            var partes = hashArmazenado.Split('.');
            if (partes.Length != 3 || !int.TryParse(partes[0], out var iteracoes))
                return false;

            var salt = Convert.FromBase64String(partes[1]);
            var hashEsperado = Convert.FromBase64String(partes[2]);

            var hashCalculado = Rfc2898DeriveBytes.Pbkdf2(senha, salt, iteracoes, HashAlgorithmName.SHA256, hashEsperado.Length);

            return CryptographicOperations.FixedTimeEquals(hashCalculado, hashEsperado);
        }
    }
}
