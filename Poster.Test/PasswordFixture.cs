using System;
using System.Linq;
using System.Security.Cryptography;
using NUnit.Framework;

namespace Poster.Test
{
    [TestFixture]
    public class PasswordFixture
    {
        [TestCase("MyPass", "KYyAgQ/QtGiAZVGtFgjZ2vF9goN3tbk8G+mp8mAQKNvI384FzIcmgpDXBPqGwqgB")]
        [TestCase("MyPass", "0YvEFAs7vfoIUBfZeD2Gw6pDW3J0evg7usy92yKwtBkG6hWP1ZPlrHCF5Vsio5K/")]
        [TestCase("MyPass", "CYBlkFYn0jQ2QJL2QP5GmHKKc9gnui5XU0kjiFrh7l5LTAWTEqhgqYMEXPba5BpY")]
        public void PowerShellHashCompareTest(string suppliedPassword, string storedBase64String)
        {
            // PowerShell:
            // https://cmatskas.com/-net-password-hashing-using-pbkdf2/
            // https://lockmedown.com/hash-right-implementing-pbkdf2-net/
            //
            //function Get-Pbkdf2Hash([string]$textToHash)
            //{
            //	[byte[]]$hashed = new-object byte[] 48
            //	[byte[]]$salt = new-object byte[] 24

            //	$rng = new-object System.Security.Cryptography.RNGCryptoServiceProvider
            //	[Void]$rng.GetBytes($salt, 0, 24)
            //	[Void]$salt.CopyTo($hashed, 0)

            //	$pbkdf2 = new-object System.Security.Cryptography.Rfc2898DeriveBytes -ArgumentList $textToHash, $salt, 1000 
            //	[Void]$pbkdf2.GetBytes(24).CopyTo($hashed, 24)

            //	return [System.Convert]::ToBase64String($hashed)
            //}

            byte[] stored = Convert.FromBase64String(storedBase64String);
            byte[] storedSalt = new byte[24];
            byte[] storedHash = new byte[24];
            
            Array.Copy(stored, 0, storedSalt, 0, 24);
            Array.Copy(stored, 24, storedHash, 0, 24);

            byte[] computedHash = new Rfc2898DeriveBytes(suppliedPassword, storedSalt, 1000).GetBytes(24);

            Assert.IsTrue(computedHash.SequenceEqual(storedHash));
        }
    }
}
