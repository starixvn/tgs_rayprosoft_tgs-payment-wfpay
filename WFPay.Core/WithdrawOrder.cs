namespace Tgs.Payment.WFPay;

internal abstract class WithdrawOrder
{
    public Guid FormId { get; init; }

    public decimal Amount { get; init; }

    public Uri CallbackUrl { get; init; } = default!;
}