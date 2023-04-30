using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Reflection;
using System.Security.Cryptography;

namespace MeowoofStore.Models.Utilities
{
    public class PasswordAndSaltProcess
    {

        public static byte[] SaltGenerator()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return salt;
        }

        public static string HashEnteredPassword(byte[] salt, string password) 
        {
            byte[] hashedByte = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            string hashPassword = Convert.ToBase64String(hashedByte);
            return hashPassword;
        }

    }
}
