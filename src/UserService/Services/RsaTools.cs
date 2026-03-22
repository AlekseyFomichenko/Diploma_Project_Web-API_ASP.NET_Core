using System.Security.Cryptography;

namespace UserService.Services
{
    public static class RsaTools
    {
        public static RSA GetPrivateKey(IConfiguration config)
        {
            var path = config["Jwt:RsaPrivateKeyPath"] ?? "rsa/private_key.pem";
            if (!Path.IsPathRooted(path))
            {
                var baseDir = Path.GetDirectoryName(typeof(RsaTools).Assembly.Location) ?? AppContext.BaseDirectory;
                path = Path.Combine(baseDir, path.Replace('/', Path.DirectorySeparatorChar));
            }
            var pem = File.ReadAllText(path, System.Text.Encoding.UTF8);
            if (pem.Length > 0 && pem[0] == '\uFEFF')
                pem = pem[1..];
            pem = pem.Trim();
            if (!pem.StartsWith("-----BEGIN", StringComparison.Ordinal))
                throw new InvalidOperationException($"JWT RSA private key file does not contain valid PEM: {path}");
            var rsa = RSA.Create();
            rsa.ImportFromPem(pem);
            return rsa;
        }
    }
}
