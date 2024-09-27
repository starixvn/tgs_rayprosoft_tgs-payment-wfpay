using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay.ViewModels;

public class WithdrawOrder
{
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    public string subject { get; set; }
    [JsonPropertyName("total_amount")]
    public required string TotalAmount { get; init; }

    public string notify_url { get; set; }

    [JsonPropertyName("merchant_order_id")]
    public required string MerchantOrderId { get; init; }

    public string status { get; set; }

    [JsonPropertyName("merchant_fee")]
    public string MerchantFee { get; init; } = default!;

    public string bank_name { get; set; }

    public string bank_province_name { get; set; }

    public string bank_city_name { get; set; }

    public string bank_account_no { get; set; }

    public string bank_account_type { get; set; }

    public string bank_account_name { get; set; }

    public object remark { get; set; }

    [JsonPropertyName("completed_at")]
    public DateTimeOffset CompletedAt { get; init; }
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }
}