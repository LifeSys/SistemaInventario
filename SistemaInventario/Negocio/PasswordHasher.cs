using System.Security.Cryptography;
using System.Text;

namespace SistemaInventario.Negocio;

public static class PasswordHasher
{
    public static string Hash(string plainText)
    {
        var bytes = Encoding.UTF8.GetBytes(plainText);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
