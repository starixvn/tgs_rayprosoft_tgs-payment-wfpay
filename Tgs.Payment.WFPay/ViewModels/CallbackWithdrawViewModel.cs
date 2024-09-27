using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay.ViewModels;

public record CallbackWithdrawViewModel
{
    [JsonPropertyName("order")]
    public required WithdrawOrder Order { get; init; }

    [JsonPropertyName("notify_type")]
    public required string NotifyType { get; init; }
}