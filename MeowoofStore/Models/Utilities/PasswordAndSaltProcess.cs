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

        public static string HashEnteredPassword<T>(T? model, byte[] salt, string passwordPropertyName) where T : class
        {
            //PropertyInfo? passwordProp = typeof(T).GetProperty(passwordPropertyName);
            var passwordProp = model.GetType().GetProperty(passwordPropertyName);

            if (passwordProp == null || !(passwordProp.GetValue(model) is string password))
            {
                throw new ArgumentException("Entity does not have a string property named 'Password'");
            }

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
