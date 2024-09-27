namespace Tgs.Payment.WFPay.Client;
public record BankWithdrawSettings(
    string MerchantNo,
    string PrivateKey,
    string PublicKey,
    Guid FormId,
    string BankName,
    string BankProvinceName,
    string BankCityName,
    string BankAccountNo,
    string BankAccountType,
    string BankAccountName) : WFPayWithdrawSettings(
        MerchantNo,
        PrivateKey,
        PublicKey,
        FormId)
{
    public override string GateWay => "Bank";
}