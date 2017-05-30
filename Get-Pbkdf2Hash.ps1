function Get-Pbkdf2Hash([string]$textToHash, [int]$cost = 10)
{
	[byte[]]$hashed = new-object byte[] 40
	[byte[]]$salt = new-object byte[] 20

	$rng = new-object System.Security.Cryptography.RNGCryptoServiceProvider
	[Void]$rng.GetBytes($salt, 0, 20)
	[Void]$salt.CopyTo($hashed, 0)

	[int]$iterations = [System.Math]::Pow(2, $cost) 

	$pbkdf2 = new-object System.Security.Cryptography.Rfc2898DeriveBytes -ArgumentList $textToHash, $salt, $iterations
	[Void]$pbkdf2.GetBytes(20).CopyTo($hashed, 20)

	$output = new-object System.Text.StringBuilder

	[Void]$output.Append("$")
	[Void]$output.Append("1a")
	[Void]$output.Append("$")
	[Void]$output.Append($cost)
	[Void]$output.Append("$")
	[Void]$output.Append([System.Convert]::ToBase64String($hashed))

	return $output.ToString()
}