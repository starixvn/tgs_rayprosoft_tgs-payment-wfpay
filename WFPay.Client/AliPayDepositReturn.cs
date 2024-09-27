namespace Tgs.Payment.WFPay.Client;
public record WFPayDepositReturn
{
    public decimal OrderAmount { get; init; }
    public Guid FormId { get; init; }

    public string Status { get; init; } = default!;

    public string Sign { get; init; } = default!;

    public string ThirdPartyOrderNo { get; init; } = default!;

    public DateTimeOffset CreatedAt { get; init; }
    public DateTimeOffset CompletedAt { get; init; }

    public decimal MerchantFee { get; init; }

    public string Currency { get; init; } = default!;
}