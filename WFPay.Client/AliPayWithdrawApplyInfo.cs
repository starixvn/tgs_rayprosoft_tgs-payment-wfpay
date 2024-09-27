namespace Tgs.Payment.WFPay.Client;
public record WFPayWithdrawApplyInfo(
    WFPayWithdrawSettings Settings,
    WFPayWithdrawResponse Response);