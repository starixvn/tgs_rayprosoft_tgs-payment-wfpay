using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Payment.WFPay.MessageQueue;

internal class DepositProcessor(
    WFPayClient wfPayClient,
    IOptions<WFPayOptions> optionsAccessor,
    ILogger<DepositProcessor> logger)
    : IMessageProcessor<DepositRequest, DepositResponse>
{
    private readonly WFPayOptions _options = optionsAccessor.Value;

    public async ValueTask<DepositResponse> HandleAsync(
        string subject,
        DepositRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{DepositApplyInfo}", data.ApplyInfo);

        var settings = JsonSerializer.Deserialize<WFPayDepositSettings>(
            data.ApplyInfo,
            WFPayUtility.SerializerOptions)!;

        var order = new DepositOrder(
            decimal.Parse(data.Amount),
            settings.FormId,
            new Uri($"{_options.CallbackBaseUrl}/deposit/callback"),
            settings.PaymentMethod);        

        var depositResponse = await wfPayClient
            .DepositAsync(settings.MerchantNo, Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKey)), order, cancellationToken)
            .ConfigureAwait(false);

        var applyInfo = new WFPayDepositApplyInfo(
            settings,
            new WFPayDepositResponse(
                depositResponse.Amount,
                depositResponse.FormId,
                depositResponse.RedirectUrl,
                depositResponse.ThirdPartyOrderNo));

        return new DepositResponse
        {
            Result = JsonSerializer.Serialize(applyInfo, WFPayUtility.SerializerOptions)
        };
    }
}