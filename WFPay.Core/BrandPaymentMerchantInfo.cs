using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay;
public record BrandPaymentMerchantInfo(
    [property: JsonPropertyName("merchantNo")] string MerchantNo,
    [property: JsonPropertyName("privateKey")] string PrivateKey);