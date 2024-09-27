using System.Text;
using System.Text.Json;
using Tgs.Payment.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Payment.WFPay.MessageQueue;

internal class GetBalanceProcessor(WFPayClient wfPayClient)
    : IMessageProcessor<PaymentBalanceRequest, PaymentBalanceResponse>
{
    public async ValueTask<PaymentBalanceResponse> HandleAsync(
        string subject,
        PaymentBalanceRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        var settings = JsonSerializer.Deserialize<BrandPaymentMerchantInfo>(
            data.BrandPaymentSettings)!;

        var result = await wfPayClient.QueryBalanceAsync(
            settings.MerchantNo,
            Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKey)),
            cancellationToken).ConfigureAwait(false);

        return new PaymentBalanceResponse
        {
            Balance = result
        };
    }
}