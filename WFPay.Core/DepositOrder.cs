namespace Tgs.Payment.WFPay;
public record DepositOrder(
    decimal Amount,
    Guid FormId,
    Uri CallbackUrl,
    string PaymentMethod);