namespace Tgs.Payment.WFPay.Client;
public record WFPayDepositResponse(
    decimal Amount,
    Guid FormId,
    Uri RedirectUrl,
    string ThirdPartyOrderNo);