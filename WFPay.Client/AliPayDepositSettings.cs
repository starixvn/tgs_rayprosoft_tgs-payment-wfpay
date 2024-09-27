namespace Tgs.Payment.WFPay.Client;
public record WFPayDepositSettings(
    string MerchantNo,
    string PrivateKey,
    string PublicKey,
    Guid FormId,
    string PaymentMethod);