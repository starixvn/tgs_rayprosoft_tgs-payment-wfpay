namespace Tgs.Payment.WFPay.Client;
public record WFPayDepositApplyInfo(
    WFPayDepositSettings Settings,
    WFPayDepositResponse Response);