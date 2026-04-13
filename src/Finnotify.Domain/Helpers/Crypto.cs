using System.Text;
using System.Security.Cryptography;

namespace Finnotify.Domain.Helpers;

public static class Crypto
{
    public static string GenerateSecureToken(int size = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(size);
        return Convert.ToBase64String(bytes);
    }

    public static string Hash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }
}
