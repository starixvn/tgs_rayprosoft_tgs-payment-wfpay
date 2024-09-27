namespace Tgs.Payment.WFPay.Client;
public abstract record WFPayWithdrawSettings(
    string MerchantNo,
    string PrivateKey,
    string PublicKey,
    Guid FormId)
{
    public abstract string GateWay { get; }
}