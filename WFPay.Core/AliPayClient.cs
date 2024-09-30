using System.Net;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;

namespace Tgs.Payment.WFPay;

public class WFPayClient(
    HttpClient httpClient,
    WFPaySigner signer,
    ILogger<WFPayClient> logger)
{
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions()
    {
        Encoder = JavaScriptEncoder.Default,
        WriteIndented = false
    };

    public async ValueTask<WFPayClientDepositResponse> DepositAsync(
        string merchantNo,
        string privateKey,
        DepositOrder order,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            account_name = merchantNo,
            merchant_order_id = order.FormId.ToString(),
            total_amount = order.Amount,
            timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            notify_url = order.CallbackUrl.AbsoluteUri,
            subject = "储值",
            payment_method = order.PaymentMethod
        };

        var json = JsonSerializer.Serialize(data, _serializerOptions);

        logger.LogInformation("DepositAsync request data:{json}", json);

        var sign = signer.CreateSign(json, privateKey);

        var dict = new Dictionary<string, string?>
        {
            ["data"] = json,
            ["signature"] = sign,
        };

        var content = new FormUrlEncodedContent(dict);

        var response = await httpClient
            .PostAsync("/merchant_api/v1/orders/payment", content, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.Created)
        {
            var node = JsonNode.Parse(body) ?? throw new ApiFailException("Deposit response can't read.");

            return new WFPayClientDepositResponse(
                Amount: decimal.Parse(node["total_amount"]!.GetValue<string>()),
                FormId: node["merchant_order_id"]!.GetValue<Guid>(),
                ThirdPartyOrderNo: node["id"]!.GetValue<string>(),
                RedirectUrl: new Uri(node["payment_url"]!.GetValue<string>()));
        }
        else if (response.StatusCode is HttpStatusCode.Forbidden)
        {
            logger.LogError("Deposit response error:{body}", body);

            throw new ApiFailException(body);
        }

        throw new ApiFailException(body);
    }

    public async ValueTask<WFPayClientWithdrawResponse> WithdrawAsync(
        string merchantNo,
        string privateKey,
        BankWithdrawOrder order,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            account_name = merchantNo,
            merchant_order_id = order.FormId.ToString(),
            total_amount = order.Amount,
            timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            notify_url = order.CallbackUrl.AbsoluteUri,
            bank_name = order.BankName,
            bank_province_name = order.BankProvinceName,
            bank_city_name = order.BankCityName,
            bank_account_no = order.BankAccountNo,
            bank_account_type = order.BankAccountType,
            bank_account_name = order.BankAccountName
        };

        var json = JsonSerializer.Serialize(data, _serializerOptions);

        logger.LogInformation("WithdrawAsync request data:{json}", json);

        var sign = signer.CreateSign(json, privateKey);

        var dict = new Dictionary<string, string?>
        {
            ["data"] = json,
            ["signature"] = sign,
        };

        var content = new FormUrlEncodedContent(dict);

        var response = await httpClient
            .PostAsync("/merchant_api/v1/orders/payment_transfer", content, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.Created)
        {
            var node = JsonNode.Parse(body) ?? throw new ApiFailException("Withdraw response can't read.");

            return new WFPayClientWithdrawResponse(
                FormId: node["merchant_order_id"]!.GetValue<Guid>(),
                ThirdPartyOrderNo: node["id"]!.GetValue<string>(),
                Amount: decimal.Parse(node["total_amount"]!.GetValue<string>()),
                Status: node["status"]!.GetValue<string>(),
                MerchantFee: node["merchant_fee"]?.GetValue<string>(),
                CreatedAt: DateTimeOffset.ParseExact(node["created_at"]!.GetValue<string>(), "yyyy-MM-ddTHH:mm:sszzz", null));
        }
        else if (response.StatusCode is HttpStatusCode.Forbidden)
        {
            logger.LogError("Withdraw response error:{body}", body);

            throw new ApiFailException(body);
        }

        throw new ApiFailException(body);
    }

    public async ValueTask<WFPayClientWithdrawResponse> WithdrawUsdtAsync(
        string merchantNo,
        string privateKey,
        UsdtWithdrawOrder order,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            account_name = merchantNo,
            merchant_order_id = order.FormId.ToString(),
            total_amount = order.Amount,
            timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"),
            //notify_url = order.CallbackUrl.AbsoluteUri,
            notify_url = "https://jrfzmv4b-7185.asse.devtunnels.ms/withdraw/callback",
            blockchain_address = order.BlockchainAddress,
        };

        var json = JsonSerializer.Serialize(data, _serializerOptions);

        logger.LogInformation("WithdrawUSDTAsync request data:{json}", json);

        var sign = signer.CreateSign(json, privateKey);

        var dict = new Dictionary<string, string?>
        {
            ["data"] = json,
            ["signature"] = sign,
        };

        var content = new FormUrlEncodedContent(dict);

        var response = await httpClient
            .PostAsync("/merchant_api/v1/orders/usdt_payment_transfer", content, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.Created)
        {
            var node = JsonNode.Parse(body) ?? throw new ApiFailException("Withdraw usdt response can't read.");

            return new WFPayClientWithdrawResponse(
                FormId: Guid.Parse(node["merchant_order_id"]!.GetValue<string>()),
                ThirdPartyOrderNo: node["id"]!.GetValue<string>(),
                Amount: decimal.Parse(node["total_amount"]!.GetValue<string>()),
                Status: node["status"]!.GetValue<string>(),
                MerchantFee: node["merchant_fee"]?.GetValue<string>(),
                CreatedAt: DateTimeOffset.ParseExact(node["created_at"]!.GetValue<string>(), "yyyy-MM-ddTHH:mm:sszzz", null));
        }
        else if (response.StatusCode is HttpStatusCode.Forbidden)
        {
            logger.LogError("Withdraw usdt response error:{body}", body);

            throw new ApiFailException(body);
        }

        throw new ApiFailException(body);
    }

    public async ValueTask<WFPayClientOrderResponse> QueryOrderAsync(
        string merchantNo,
        string privateKey,
        Guid formId,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            account_name = merchantNo,
            merchant_order_id = formId.ToString(),
            timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
        };

        var json = JsonSerializer.Serialize(data, _serializerOptions);

        var sign = signer.CreateSign(json, privateKey);

        var dict = new Dictionary<string, string>
        {
            ["data"] = json,
            ["signature"] = sign,
        };

        var content = new FormUrlEncodedContent(dict);

        var response = await httpClient
            .PostAsync($"/merchant_api/v1/orders/query", content, cancellationToken)
            .ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (response.StatusCode is HttpStatusCode.Created)
        {
            var node = JsonNode.Parse(body) ?? throw new ApiFailException("Query order response can't read.");

            DateTimeOffset? completedAt = node["completed_at"]?.GetValue<string>() is null
                    ? null
                    : DateTimeOffset.ParseExact(node["completed_at"]!.GetValue<string>(), "yyyy-MM-ddTHH:mm:sszzz", null);

            WFPayClientOrderPaymentInfo? paymentInfo = node["payment_info"] is null
                    ? null
                    : new WFPayClientOrderPaymentInfo(
                        node["payment_info"]!["account_name"]!.GetValue<string>(),
                        node["payment_info"]!["surname"]!.GetValue<string>());

            return new WFPayClientOrderResponse(
                ThirdPartyOrderNo: node["id"]!.GetValue<string>(),
                Amount: decimal.Parse(node["total_amount"]!.GetValue<string>()),
                Status: node["status"]!.GetValue<string>(),
                CompletedAt: completedAt,
                MerchantFee: decimal.Parse(node["merchant_fee"]?.GetValue<string>() ?? "0"),
                PaymentInfo: paymentInfo);
        }
        else if (response.StatusCode is HttpStatusCode.Forbidden)
        {
            logger.LogError("Withdraw response error:{body}", body);

            throw new ApiFailException(body);
        }
        throw new ApiFailException(body);
    }

    public async ValueTask<decimal> QueryBalanceAsync(
        string merchantNo,
        string privateKey,
        CancellationToken cancellationToken = default)
    {
        var data = new
        {
            account_name = merchantNo,
            timestamp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz")
        };

        var json = JsonSerializer.Serialize(data, _serializerOptions);

        var sign = signer.CreateSign(json, privateKey);

        var dict = new Dictionary<string, string>
        {
            ["data"] = json,
            ["signature"] = sign,
        };

        var content = new FormUrlEncodedContent(dict);

        var response = await httpClient
            .PostAsync($"/merchant_api/v1/balances/query", content, cancellationToken)
            .ConfigureAwait(false);

        _ = response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        var node = JsonNode.Parse(body) ?? throw new ApiFailException("Query balance response can't read.");

        return decimal.Parse(node["balance"]!.GetValue<string>());
    }
}