using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay.ViewModels;

public record DepositOrder
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }
    public string subject { get; init; }
    [JsonPropertyName("total_amount")]
    public required string TotalAmount { get; init; }
    public string notify_url { get; init; }
    public object return_url { get; init; }
    [JsonPropertyName("merchant_order_id")]
    public required string MerchantOrderId { get; init; }
    public string status { get; init; }
    public string payment_url { get; init; }
    [JsonPropertyName("merchant_fee")]
    public string MerchantFee { get; init; } = default!;
    public string confirmation_code { get; init; }
    public object remark { get; init; }
    public object bank_account { get; init; }
    public object qrcode_image_url { get; init; }
    public object qrcode_url { get; init; }
    public object payment_image_url { get; init; }
    public object[] supplement_orders { get; init; }
    public string supplement_orders_status { get; init; }
    public DepositPaymentInfo payment_info { get; init; }
    [JsonPropertyName("currency")]
    public string Currency { get; init; } = default!;
    [JsonPropertyName("completed_at")]
    public DateTimeOffset CompletedAt { get; init; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}