namespace Tgs.Payment.WFPay;
public class WFPayOptions
{
    public required Uri CallbackBaseUrl { get; set; }

    public required Uri PaymentApiUrl { get; set; }

    public required Uri PaymentServiceBaseUrl { get; set; }
}
