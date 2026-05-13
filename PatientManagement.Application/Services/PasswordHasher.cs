using System.Security.Cryptography;
using System.Text;
using PatientManagement.Application.Interfaces;

namespace PatientManagement.Application.Services;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 100_000;

    public string HashPassword(string password)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));

        using var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA512);
        var key = algorithm.GetBytes(KeySize);
        var salt = algorithm.Salt;

        var hashBytes = new byte[SaltSize + KeySize];
        Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
        Buffer.BlockCopy(key, 0, hashBytes, SaltSize, KeySize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        if (password == null) throw new ArgumentNullException(nameof(password));
        if (hashedPassword == null) throw new ArgumentNullException(nameof(hashedPassword));

        var hashBytes = Convert.FromBase64String(hashedPassword);
        var salt = new byte[SaltSize];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);

        using var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA512);
        var key = algorithm.GetBytes(KeySize);

        for (var i = 0; i < KeySize; i++)
        {
            if (hashBytes[i + SaltSize] != key[i])
            {
                return false;
            }
        }

        return true;
    }
}
