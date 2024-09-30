using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.WFPay;

public class WFPaySigner(ILogger<WFPaySigner> logger)
{
    public string CreateSign(
        string source,
        string privateKey)
    {
        var inputBytes = Encoding.UTF8.GetBytes(source);
        var inputHash = SHA256.HashData(inputBytes);
        var privateKeyBytes = Convert.FromBase64String(
            privateKey.Replace("-----BEGIN RSA PRIVATE KEY-----", string.Empty)
            .Replace("-----END RSA PRIVATE KEY-----", string.Empty)
            .Replace("\n", string.Empty)
            .Replace("\r",string.Empty));

        var rsa = RSAUtils.DecodeRSAPrivateKey(privateKeyBytes);

        var output = rsa.SignHash(inputHash, "SHA256");

        var result = Convert.ToBase64String(output);

        logger.LogInformation("CreateSign:{result}", result);

        return result;
    }

    public bool VerifySign(
        string data,
        string sign,
        string publicKey)
    {
        var rsa = new RSACryptoServiceProvider();

        var publicKeyBase64 = publicKey
            .Replace("-----BEGIN PUBLIC KEY-----", "")
            .Replace("-----END PUBLIC KEY-----", "")
            .Replace("\n", string.Empty);

        byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);

        rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        var shaOID = CryptoConfig.MapNameToOID("SHA256")!;

        var buffer = Encoding.UTF8.GetBytes(data);
        var signature = Convert.FromBase64String(sign);

        return rsa.VerifyData(buffer, shaOID, signature);
    }
}