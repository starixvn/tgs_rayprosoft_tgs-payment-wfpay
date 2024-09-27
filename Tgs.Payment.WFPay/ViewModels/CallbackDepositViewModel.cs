using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay.ViewModels;

public record CallbackDepositViewModel
{
    [JsonPropertyName("order")]
    public required DepositOrder Order { get; init; }

    [JsonPropertyName("notify_type")]
    public required string NotifyType { get; init; }
}