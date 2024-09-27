using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Clients;

namespace Tgs.Payment.WFPay;
public record WithdrawForm(
    Guid Id,
    string? UserId,
    UserType UserType,
    Guid BrandId,
    Guid? BrandPaymentId,
    decimal Amount,
    PaymentFlowState Status,
    Guid BankInfoId,
    string? BankInfo,
    WFPayWithdrawApplyInfo? ApplyInfo,
    string? PaymentReturn = null);