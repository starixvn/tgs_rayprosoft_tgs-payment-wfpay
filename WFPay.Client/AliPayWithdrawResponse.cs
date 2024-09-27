namespace Tgs.Payment.WFPay.Client;
public record WFPayWithdrawResponse(
    Guid FormId,
    string ThirdPartyOrderNo,
    string Status,
    string? MerchantFee,
    DateTimeOffset CreatedAt);