namespace Tgs.Payment.WFPay.Client;
public record UsdtWithdrawSettings(
    string MerchantNo,
    string PrivateKey,
    string PublicKey,
    Guid FormId,
    string BlockchainAddress) : WFPayWithdrawSettings(
        MerchantNo,
        PrivateKey,
        PublicKey,
        FormId)
{
    public override string GateWay => "Usdt";
}