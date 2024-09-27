using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Clients;

namespace Tgs.Payment.WFPay;
public record DepositForm(
    Guid Id,
    string PlayerCode,
    Guid BrandId,
    Guid BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    WFPayDepositApplyInfo? ApplyInfo,
    string? PaymentReturn = null);