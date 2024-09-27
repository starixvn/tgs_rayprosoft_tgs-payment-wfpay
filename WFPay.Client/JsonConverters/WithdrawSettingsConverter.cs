using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Tgs.Payment.WFPay.Client.JsonConverters;
internal class WithdrawSettingsConverter : JsonConverter<WFPayWithdrawSettings>
{
    public override WFPayWithdrawSettings? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var jsonDoc = JsonDocument.ParseValue(ref reader);

        var gateway = jsonDoc.RootElement.GetProperty("gateWay").GetString();

        using var ms = new MemoryStream();

        var writer = new Utf8JsonWriter(ms, new JsonWriterOptions { Indented = false });
        jsonDoc.WriteTo(writer);
        writer.Flush();

        var json = Encoding.UTF8.GetString(ms.ToArray());

        return gateway switch
        {
            "Usdt" => JsonSerializer.Deserialize<UsdtWithdrawSettings>(json, options),
            "Bank" => JsonSerializer.Deserialize<BankWithdrawSettings>(json, options),            
            _ => throw new JsonException("Can't read withdraw settings.")
        };
    }

    public override void Write(Utf8JsonWriter writer, WFPayWithdrawSettings value, JsonSerializerOptions options) 
        => JsonDocument.Parse(JsonSerializer.Serialize(
            value,
            value.GetType(),
            options)).WriteTo(writer);
}
