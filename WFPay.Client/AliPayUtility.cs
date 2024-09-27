using System.Text.Json;
using System.Text.Json.Serialization;
using Tgs.Payment.WFPay.Client.JsonConverters;

namespace Tgs.Payment.WFPay.Client;

public static class WFPayUtility
{
    public static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        Converters = {
            new WithdrawSettingsConverter()
        }
    };
}