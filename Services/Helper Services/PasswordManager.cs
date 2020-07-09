using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Services.Helper_Services
{
    public class PasswordManager : IPasswordManager
    {

        private const int keySize = 18;
        private const int hashIteration = 10000;

        private byte[] CreateSalt()
        {
            byte[] ranBytes = new byte[18];
            using (var generate = RandomNumberGenerator.Create())
            {
                generate.GetBytes(ranBytes);
                return ranBytes;
            }
        }

        public string HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, CreateSalt(), hashIteration, HashAlgorithmName.SHA512))
            {
                var key = Convert.ToBase64String(algorithm.GetBytes(keySize));
                var salt = Convert.ToBase64String(algorithm.Salt);
                var iter = algorithm.IterationCount;

                return $"{ iter}.{ salt}.{ key}";
            }
        }

        public bool VerifyPassword(string hashedPassword, string password)
        {
            var passPart = hashedPassword.Split('.', 3);

            var iteration = Convert.ToInt32(passPart[0]);
            var salt = Convert.FromBase64String(passPart[1]);
            var key = Convert.FromBase64String(passPart[2]);

            using (var algorithm = new Rfc2898DeriveBytes(password, salt, iteration, HashAlgorithmName.SHA512))
            {
                var keyCheck = algorithm.GetBytes(keySize);
                bool isVerified = keyCheck.SequenceEqual(key);

                return isVerified;
            }
        }
    }

    public interface IPasswordManager
    {
        public string HashPassword(string password);
        public bool VerifyPassword(string hashedPassword, string password);
    }
}
