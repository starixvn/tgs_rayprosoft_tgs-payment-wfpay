namespace Tgs.Payment.WFPay;
public record WFPayClientWithdrawResponse(
    Guid FormId,
    string ThirdPartyOrderNo,
    decimal Amount,
    string Status,
    string? MerchantFee,
    DateTimeOffset CreatedAt);