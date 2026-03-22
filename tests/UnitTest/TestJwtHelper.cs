using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace UnitTest
{
    public static class TestJwtHelper
    {
        public static string CreateToken(int userId = 1, string role = "User")
        {
            var baseDir = Path.GetDirectoryName(typeof(TestJwtHelper).Assembly.Location) ?? AppContext.BaseDirectory;
            var keyPath = Path.Combine(baseDir, "rsa", "private_key.pem");
            if (!File.Exists(keyPath))
                throw new FileNotFoundException("Private key not found for test JWT", keyPath);
            var pem = File.ReadAllText(keyPath, System.Text.Encoding.UTF8);
            if (pem.Length >= 1 && pem[0] == '\uFEFF')
                pem = pem[1..];
            pem = pem.Trim();
            var rsa = RSA.Create();
            rsa.ImportFromPem(pem);
            var key = new RsaSecurityKey(rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256Signature);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Role, role)
            };
            var token = new JwtSecurityToken(
                "https://localhost:7049",
                "https://localhost:7049",
                claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
