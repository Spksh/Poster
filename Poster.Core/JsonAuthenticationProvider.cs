using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Poster.MetaWeblogApi;

namespace Poster.Core
{
    public class JsonAuthenticationProvider : IAuthenticationProvider
    {
        private const string ConfigurationSectionName = "authentication";

        public Func<Task<AuthenticationConfiguration>> GetConfigurationAsync { get; set; }

        public JsonAuthenticationProvider()
        {
            
        }

        public JsonAuthenticationProvider(IConfigurationProvider configurationProvider)
        {
            GetConfigurationAsync = () => configurationProvider.GetConfigurationSectionAsync<AuthenticationConfiguration>(ConfigurationSectionName);
        }

        public JsonAuthenticationProvider With(IConfigurationProvider configurationProvider)
        {
            GetConfigurationAsync = () => configurationProvider.GetConfigurationSectionAsync<AuthenticationConfiguration>(ConfigurationSectionName);

            return this;
        }

        public async Task<bool> IsAuthenticatedAsync(string username, string password)
        {
            User user = (await GetConfigurationAsync()).Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                return false;
            }

            string[] segments = user.Password.Split(new char[] { '$' }, StringSplitOptions.RemoveEmptyEntries);

            string version = segments[0]; // Unused until we change the hash function (e.g. by adding a secret verification)
            int cost = int.Parse(segments[1]); // interation count is 2 ^ cost, e.g. 10 is 1024, 20 is 1048576
            int iterations = (int)Math.Pow(2, cost);

            byte[] saltPlusHash = Convert.FromBase64String(segments[2]);
            byte[] salt = new byte[20];
            byte[] hash = new byte[20];

            Array.Copy(saltPlusHash, 0, salt, 0, 20);
            Array.Copy(saltPlusHash, 20, hash, 0, 20);

            byte[] computedHash = new Rfc2898DeriveBytes(password, salt, iterations).GetBytes(20);

            return computedHash.SequenceEqual(hash);
        }
    }
}
