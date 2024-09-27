namespace Tgs.Payment.WFPay;

internal class UsdtWithdrawOrder : WithdrawOrder
{
    public string BlockchainAddress { get; init; } = default!;
}