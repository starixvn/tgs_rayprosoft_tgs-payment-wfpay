using System.Text.Json;

namespace Tgs.Payment.WFPay;
public record WFPayClientOrderResponse(
    string ThirdPartyOrderNo,
    decimal Amount,
    string Status,
    DateTimeOffset? CompletedAt,
    decimal MerchantFee,
    WFPayClientOrderPaymentInfo? PaymentInfo);