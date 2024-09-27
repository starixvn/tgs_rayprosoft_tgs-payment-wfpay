using System.Text.Json;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.WFPay.ApplicationServices;

internal class GetWithdrawFormQueryService(
    IPaymentServiceClient paymentServiceClient) : IGetService<GetWithdrawFormQuery, WithdrawForm?>
{
    public async ValueTask<WithdrawForm?> GetAsync(GetWithdrawFormQuery query, CancellationToken cancellationToken = default)
    {
        var data = await paymentServiceClient
            .GetWithdrawFormAsync(query.FormId, cancellationToken)
            .ConfigureAwait(false);

        return data == null
            ? null
            : new WithdrawForm(
                data.Id,
                data.UserId,
                data.UserType,
                data.BrandId,
                data.BrandPaymentId,
                data.Amount,
                data.Status,
                data.BankInfoId,
                data.BankInfo,
                string.IsNullOrEmpty(data.ApplyInfo)
                ? null
                : JsonSerializer.Deserialize<WFPayWithdrawApplyInfo>(data.ApplyInfo, WFPayUtility.SerializerOptions),
                    data.PaymentReturn);
    }
}