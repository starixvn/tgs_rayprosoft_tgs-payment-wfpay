namespace Tgs.Payment.WFPay;

public class BankWithdrawOrder : WithdrawOrder
{
    /// <summary>
    /// 银行名称
    /// </summary>
    public string BankName { get; init; } = default!;

    /// <summary>
    /// 银行省份
    /// </summary>
    public string BankProvinceName { get; init; } = default!;

    /// <summary>
    /// 银行城市
    /// </summary>
    public string BankCityName { get; init; } = default!;

    /// <summary>
    /// 银行帐户号码
    /// </summary>
    public string BankAccountNo { get; init; } = default!;

    /// <summary>
    /// 银行帐户类型
    /// </summary>
    public string BankAccountType { get; init; } = default!;

    /// <summary>
    /// 银行帐户名
    /// </summary>
    public string BankAccountName { get; init; } = default!;
}