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
            Random rand = new Random();
            char[] salt = new char[25];

            //Generate random ASCII character for each index of salt
            for (int i = 0; i < salt.Length; i++)
            {
                salt[i] = (char)rand.Next(32, 126);
            }
            return new string(salt);
        }
    }
}
