using System.Text;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace NextHave.BL.Extensions
{
    public static class PasswordExtensions
    {
        const int HashSize = 32;
        const int Iterations = 4;
        const int Parallelism = 4;
        const int MemorySize = 65536;

        public static string HashPassword(this string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 65536,
                Iterations = 4
            };

            var hash = argon2.GetBytes(32);

            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        public static bool VerifyPassword(this string password, string stored)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(stored))
                return false;

            var parts = stored.Split(':');
            if (parts.Length != 2)
                return false;

            try
            {
                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                byte[] computedHash = ComputeArgon2Hash(password, salt);
                
                string hashString = Convert.ToBase64String(computedHash);
                return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
            }
            catch
            {
                // In caso di errore di formato o conversione
                return false;
            }
        }

        static byte[] ComputeArgon2Hash(string password, byte[] salt)
        {
            using var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = Parallelism,
                MemorySize = MemorySize,
                Iterations = Iterations
            };

            return argon2.GetBytes(HashSize);
        }
    }
}