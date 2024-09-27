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
public class DepositController(ILogger<DepositController> logger) : ControllerBase
{
    [HttpPost("callback")]
    [Consumes(MediaTypeNames.Application.FormUrlEncoded)]
    public async Task<string> DepositCallbackAsync(
       [FromServices] IGetService<GetDepositFormQuery, DepositForm?> getService,
       [FromServices] IPaymentServiceClient paymentServiceClient,
       [FromServices] IPaymentClient paymentClient,
       [FromServices] WFPaySigner signer,
       [FromForm] IFormCollection forms)
    {
        logger.LogInformation("{forms}", forms);

        var data = forms["data"].ToString();
        var sign = forms["signature"].ToString();

        var source = JsonSerializer.Deserialize<CallbackDepositViewModel>(data)!;
        var formId = Guid.Parse(source.Order.MerchantOrderId);

        var depositForm = await getService
            .GetAsync(new(formId), HttpContext.RequestAborted)
            .ConfigureAwait(false);

        if (depositForm == null)
        {
            logger.LogWarning("Can't found deposit form({id}).", formId);
            return "fail";
        }

        var publicKey = Encoding.UTF8.GetString(Convert.FromBase64String(depositForm.ApplyInfo!.Settings.PublicKey));        

        if (signer.VerifySign(data, sign, publicKey) is false)
        {
            logger.LogWarning("Sign invalid. input: {sign}", sign);
            return "fail";
        }

        var paymentReturn = new WFPayDepositReturn
        {
            OrderAmount = decimal.Parse(source.Order.TotalAmount),
            FormId = formId,
            Status = source.Order.status,
            Sign = sign,
            ThirdPartyOrderNo = source.Order.Id,
            CreatedAt = source.Order.CreatedAt,
            CompletedAt = source.Order.CompletedAt,
            MerchantFee = decimal.Parse(source.Order.MerchantFee),
            Currency = source.Order.Currency,
        };

        var fsm = PaymentStateMachineFactory.CreateDepositStateMachine(depositForm.Status);

        switch (paymentReturn.Status)
        {
            case "completed"://成功
                if (fsm.CanFire(PaymentTrigger.PaymentDepositCompleted))
                {
                    await fsm.FireAsync(PaymentTrigger.PaymentDepositCompleted).ConfigureAwait(false);

                    await paymentServiceClient.SubmitDepositFormStatusAsync(
                        depositForm.Id,
                        paymentReturn: JsonSerializer.Serialize(paymentReturn, WFPayUtility.SerializerOptions),
                        cancellationToken: HttpContext.RequestAborted).ConfigureAwait(false);

                    await paymentClient.ApproveDepositAsync(
                        depositForm.Id,
                        depositForm.BrandId,
                        paymentReturn.OrderAmount,
                        HttpContext.RequestAborted).ConfigureAwait(false);
                }

                break;

            default:
                break;
        }

        return "success";
    }
}