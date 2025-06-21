using System.Text;
using System.Security.Cryptography;
using Konscious.Security.Cryptography;

namespace NextHave.BL.Extensions
{
    public static class PasswordExtensions
    {
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
            var parts = stored.Split(':');
            if (parts.Length != 2)
                return false;

            var salt = Convert.FromBase64String(parts[0]);
            var storedHash = Convert.FromBase64String(parts[1]);

            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                MemorySize = 65536,
                Iterations = 4
            };

            var computedHash = argon2.GetBytes(32);

            return CryptographicOperations.FixedTimeEquals(storedHash, computedHash);
        }
    }
}