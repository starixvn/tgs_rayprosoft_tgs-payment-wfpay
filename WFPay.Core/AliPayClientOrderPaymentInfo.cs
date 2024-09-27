using System.ComponentModel;

namespace Tgs.Payment.WFPay;
public record WFPayClientOrderPaymentInfo(
    string AccountName,
    [Description("收款人姓氏首字")]
    string SurName);