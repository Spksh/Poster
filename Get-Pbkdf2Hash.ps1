function Get-Pbkdf2Hash([string]$textToHash)
{
	[byte[]]$hashed = new-object byte[] 48
	[byte[]]$salt = new-object byte[] 24

	$rng = new-object System.Security.Cryptography.RNGCryptoServiceProvider
	[Void]$rng.GetBytes($salt, 0, 24)
	[Void]$salt.CopyTo($hashed, 0)

	$pbkdf2 = new-object System.Security.Cryptography.Rfc2898DeriveBytes -ArgumentList $textToHash, $salt, 1000 
	[Void]$pbkdf2.GetBytes(24).CopyTo($hashed, 24)

	return [System.Convert]::ToBase64String($hashed)
}