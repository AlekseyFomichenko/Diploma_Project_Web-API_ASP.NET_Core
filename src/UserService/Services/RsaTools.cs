using System.Security.Cryptography;

namespace UserService.Services
{
    public static class RsaTools
    {
        public static RSA GetPrivateKey(IConfiguration config)
        {
            var path = config["Jwt:RsaPrivateKeyPath"] ?? "rsa/private_key.pem";
            var f = File.ReadAllText(path);
            var rsa = RSA.Create();
            rsa.ImportFromPem(f);
            return rsa;
        }
    }
}
