using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Tgs.Payment.WFPay.ApplicationServices;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.WFPay.ViewModels;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;

namespace Tgs.Payment.WFPay.Controllers;

[Route("[controller]")]
[ApiController]
public class WithdrawController(ILogger<WithdrawController> logger) : ControllerBase
{
    [HttpPost("callback")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    public async Task<string> WithdrawCallbackAsync(
        [FromServices] IGetService<GetWithdrawFormQuery, WithdrawForm?> getService,
        [FromServices] IPaymentServiceClient paymentServiceClient,
        [FromServices] IPaymentClient paymentClient,
        [FromServices] WFPaySigner signer,
        [FromForm] IFormCollection forms)
    {
        logger.LogInformation(message: "{forms}", forms);

        var data = forms["data"].ToString();
        var sign = forms["signature"].ToString();

        var source = JsonSerializer.Deserialize<CallbackWithdrawViewModel>(data)!;
        var formId = Guid.Parse(source.Order.MerchantOrderId);

        var withdrawForm = await getService
            .GetAsync(new(formId), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (withdrawForm == null)
        {
            logger.LogWarning("Can't found withdraw form({id}).", formId);
            return "fail";
        }

        var publicKey = Encoding.UTF8.GetString(Convert.FromBase64String(withdrawForm.ApplyInfo!.Settings.PublicKey));

        if (signer.VerifySign(data, sign, publicKey) is false)
        {
            logger.LogWarning("Sign invalid. input: {sign}", sign);
            return "fail";
        }

        var paymentReturn = new WFPayWithdrawReturn()
        {
            OrderAmount = decimal.Parse(source.Order.TotalAmount),
            FormId = formId,
            Status = source.Order.status,
            Sign = sign,
            ThirdPartyOrderNo = source.Order.Id,
            CreatedAt = source.Order.CreatedAt,
            CompletedAt = source.Order.CompletedAt,
            MerchantFee = decimal.Parse(source.Order.MerchantFee),
        };

        var fsm = PaymentStateMachineFactory.CreateWithdrawStateMachine(withdrawForm.Status);

        switch (paymentReturn.Status)
        {
            case "completed"://交易完成
                if (fsm.CanFire(PaymentTrigger.PaymentWithdrawCompleted))
                {
                    await fsm.FireAsync(PaymentTrigger.PaymentWithdrawCompleted).ConfigureAwait(false);

                    await paymentServiceClient.SubmitWithdrawFormStatusAsync(
                        withdrawForm.Id,
                        paymentReturn: JsonSerializer.Serialize(paymentReturn, WFPayUtility.SerializerOptions)).ConfigureAwait(false);

                    await paymentClient
                        .PaidWithdrawAsync(
                            withdrawForm.Id,
                            withdrawForm.BrandId,
                            HttpContext.RequestAborted)
                        .ConfigureAwait(false);
                }

                break;

            case "failed"://失敗
                if (fsm.CanFire(PaymentTrigger.Reject))
                {
                    await fsm.FireAsync(PaymentTrigger.Reject).ConfigureAwait(false);

                    await paymentServiceClient.SubmitWithdrawFormStatusAsync(
                        withdrawForm.Id,
                        paymentReturn: JsonSerializer.Serialize(paymentReturn, WFPayUtility.SerializerOptions)).ConfigureAwait(false);

                    await paymentClient
                        .RejectWithdrawAsync(withdrawForm.Id, withdrawForm.BrandId)
                        .ConfigureAwait(false);
                }

                break;
        }

        return "success";
    }
}