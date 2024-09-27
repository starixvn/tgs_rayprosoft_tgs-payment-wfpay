using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;
using Valhalla.MessageQueue;

namespace Tgs.Payment.WFPay.MessageQueue;

internal class WithdrawProcessor(
    WFPayClient wfPayClient,
    IOptions<WFPayOptions> optionsAccessor,
    IPaymentClient paymentClient,
    ILogger<WithdrawProcessor> logger)
    : IMessageProcessor<WithdrawRequest, WithdrawResponse>
{
    private readonly WFPayOptions _options = optionsAccessor.Value;

    public async ValueTask<WithdrawResponse> HandleAsync(
        string subject,
        WithdrawRequest data,
        IEnumerable<MessageHeaderValue>? headerValues,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("{applyInfo}", data.ApplyInfo);

        var settings = JsonSerializer.Deserialize<WFPayWithdrawSettings>(data.ApplyInfo, WFPayUtility.SerializerOptions)!;

        var amount = decimal.Parse(data.Amount);

        var thirdPartyOrderNo = string.Empty;

        WFPayWithdrawApplyInfo applyInfo = default!;

        switch (settings.GateWay)
        {
            case "Bank":

                var bankSettings = (BankWithdrawSettings)settings;

                var bankOrder = new BankWithdrawOrder
                {
                    Amount = amount,
                    FormId = settings.FormId,
                    CallbackUrl = new Uri($"{_options.CallbackBaseUrl}/withdraw/callback"),
                    BankAccountName = bankSettings.BankAccountName,
                    BankAccountNo = bankSettings.BankAccountNo,
                    BankAccountType = bankSettings.BankAccountType,
                    BankCityName = bankSettings.BankCityName,
                    BankName = bankSettings.BankName,
                    BankProvinceName = bankSettings.BankProvinceName,
                };

                var withdrawBankResponse = await wfPayClient
                    .WithdrawAsync(settings.MerchantNo, Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKey)), bankOrder, cancellationToken)
                    .ConfigureAwait(false);

                thirdPartyOrderNo = withdrawBankResponse.ThirdPartyOrderNo;

                applyInfo = new WFPayWithdrawApplyInfo(
                    settings,
                    new WFPayWithdrawResponse(
                        withdrawBankResponse.FormId,
                        thirdPartyOrderNo,
                        withdrawBankResponse.Status,
                        withdrawBankResponse.MerchantFee,
                        withdrawBankResponse.CreatedAt));

                break;

            case "Usdt":

                var usdtSettings = (UsdtWithdrawSettings)settings;

                var usdtOrder = new UsdtWithdrawOrder
                {
                    Amount = amount,
                    FormId = settings.FormId,
                    CallbackUrl = new Uri($"{_options.CallbackBaseUrl}/withdraw/callback"),
                    BlockchainAddress = usdtSettings.BlockchainAddress,
                };

                var withdrawUsdtResponse = await wfPayClient
                    .WithdrawUsdtAsync(settings.MerchantNo, Encoding.UTF8.GetString(Convert.FromBase64String(settings.PrivateKey)), usdtOrder, cancellationToken)
                    .ConfigureAwait(false);

                thirdPartyOrderNo = withdrawUsdtResponse.ThirdPartyOrderNo;

                applyInfo = new WFPayWithdrawApplyInfo(
                    settings,
                    new WFPayWithdrawResponse(
                        withdrawUsdtResponse.FormId,
                        thirdPartyOrderNo,
                        withdrawUsdtResponse.Status,
                        withdrawUsdtResponse.MerchantFee,
                        withdrawUsdtResponse.CreatedAt));

                break;

            default:
                break;
        }

        await paymentClient
            .SaveWithdrawFormThirdPartyOrderNoAsync(
                settings.FormId,
                thirdPartyOrderNo,
                cancellationToken)
            .ConfigureAwait(false);

        return new WithdrawResponse
        {
            Result = JsonSerializer.Serialize(applyInfo, WFPayUtility.SerializerOptions)
        };
    }
}