using System;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Poster.Test
{
    [TestFixture]
    public class PasswordFixture
    {
        [TestCase("MyPass", "$1a$10$ETAwacUaotSTWIHOTf8ELgLZI1mnvMphW9NKqxmwLZHltdN9SDbMVg==")]
        [TestCase("MyPass", "$1a$10$lxVnWag/X5bLfqOCHwK7tWMQIwM79iUFAoRjZ/IZTnbZ0CIcP9ePJw==")]
        [TestCase("MyPass", "$1a$16$40MvhSEMaChBhfXYPGwCbn9l1U+I+VUa7VK3wGfEpN2ciQ9hWgIx/w==")]
        [TestCase("MyPass", "$1a$20$CvlsMnK+1liIKOURhARJEABLPw70lZ4fTLSgC5MFz8ENaamDiBl94g==")]
        public void PowerShellHashCompareTest(string suppliedPassword, string storedBase64String)
        {
            // PowerShell:
            // https://cmatskas.com/-net-password-hashing-using-pbkdf2/
            // https://lockmedown.com/hash-right-implementing-pbkdf2-net/
            // https://www.owasp.org/index.php/Using_Rfc2898DeriveBytes_for_PBKDF2
            //
            //function Get-Pbkdf2Hash([string]$textToHash, [int]$cost = 10)
            //{
            //	[byte[]]$hashed = new-object byte[] 40
            //	[byte[]]$salt = new-object byte[] 20

            //	$rng = new-object System.Security.Cryptography.RNGCryptoServiceProvider
            //	[Void]$rng.GetBytes($salt, 0, 20)
            //	[Void]$salt.CopyTo($hashed, 0)

            //	[int]$iterations = [System.Math]::Pow(2, $cost) 

            //	$pbkdf2 = new-object System.Security.Cryptography.Rfc2898DeriveBytes -ArgumentList $textToHash, $salt, $iterations
            //	[Void]$pbkdf2.GetBytes(20).CopyTo($hashed, 20)

            //	$output = new-object System.Text.StringBuilder

            //	[Void]$output.Append("$")
            //	[Void]$output.Append("1a")
            //	[Void]$output.Append("$")
            //	[Void]$output.Append($cost)
            //	[Void]$output.Append("$")
            //	[Void]$output.Append([System.Convert]::ToBase64String($hashed))

            //	return $output.ToString()
            //}

            string[] segments = storedBase64String.Split(new char[] {'$'}, StringSplitOptions.RemoveEmptyEntries);

            string version = segments[0]; // Unused until we change the hash function (e.g. by adding a secret verification)
            int cost = int.Parse(segments[1]); // interation count is 2 ^ cost, e.g. 10 is 1024, 20 is 1048576
            int iterations = (int) Math.Pow(2, cost);

            byte[] saltPlusHash = Convert.FromBase64String(segments[2]);
            byte[] salt = new byte[20];
            byte[] hash = new byte[20];
            
            Array.Copy(saltPlusHash, 0, salt, 0, 20);
            Array.Copy(saltPlusHash, 20, hash, 0, 20);

            byte[] computedHash = new Rfc2898DeriveBytes(suppliedPassword, salt, iterations).GetBytes(20);

            Assert.IsTrue(computedHash.SequenceEqual(hash));
        }
    }
}
