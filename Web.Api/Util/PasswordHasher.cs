using System.Security.Cryptography;
using System.Text;

namespace Web.Api.Util
{
    public class PasswordHasher
    {
        public byte[] GenerateHash(string password, string salt)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                string saltPsw = salt + password;
                byte[] hash = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(saltPsw));
                return hash;
            }
        }

        public string GenerateSalt()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(24));
        }
    }
}
