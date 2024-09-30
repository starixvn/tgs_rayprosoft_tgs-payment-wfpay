namespace Tgs.Payment.WFPay;

public class UsdtWithdrawOrder : WithdrawOrder
{
    public string BlockchainAddress { get; init; } = default!;
}