using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.Payment.WFPay;

namespace WFPay.Core.UnitTests;

public class WFPayClientTests
{
    [Fact]
    public async Task 代收()
    {
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");
        var signer = new WFPaySigner(NullLogger<WFPaySigner>.Instance);

        var sut = new WFPayClient(httpClient, signer, NullLogger<WFPayClient>.Instance);

        var merchantNo = "merchantNo";
        var privateKey = """
    -----BEGIN RSA PRIVATE KEY-----
    MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
    zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
    GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
    JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
    q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
    EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
    e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
    Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
    6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
    CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
    IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
    zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
    ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
    NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
    DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
    jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
    kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
    oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
    ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
    WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
    Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
    dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
    kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
    HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
    8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
    -----END RSA PRIVATE KEY-----
    """;

        var order = new DepositOrder(
            1000m, 
            Guid.Parse("ee051ad1-05cb-4198-9aa1-e44d490a449d"), 
            new Uri("https://jrfzmv4b-7185.asse.devtunnels.ms/deposit/callback"),
            "alipay");

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/orders/payment")
            .Respond(HttpStatusCode.Created, new StringContent("""
                {
                    "id": "5dac6bd0-66ee-43e9-bf01-595a2f47f5f5",
                    "subject": "储值",
                    "total_amount": "1000.0",
                    "notify_url": "https://jrfzmv4b-7185.asse.devtunnels.ms/deposit/callback",
                    "return_url": null,
                    "merchant_order_id": "ee051ad1-05cb-4198-9aa1-e44d490a449d",
                    "status": "init",
                    "payment_url": "https://gnngbwtf.com/orders/5dac6bd0-66ee-43e9-bf01-595a2f47f5f5?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6IjVkYWM2YmQwLTY2ZWUtNDNlOS1iZjAxLTU5NWEyZjQ3ZjVmNSJ9.UfSM2M9HqZl871I2l7KRonBa3uG4MfljeK32KB6AfymynaKhiZNPslbF7Wl0YC6wqH8G1Ad5RqH3OLHwOx-JY87aZyx64abC_qEOhkdyuuMUbYHplULegAyqSh2EpXVEJTr8WdSkqpCm9ofmGhHzBygEdJYi0K8oyDWo6tgYfxCduEro2ADQ6QKCwRh3Zl1sDVGqWWFlXqoFDT4KYSs_9I2EV1-KIyPcRmB0ncM2mZq126x5mAMS0ZJucwKIKeiY7AmeaTKC3gVP6fZHITCnMSTePl9Ut8ebALACyQIrAKnu7qxwWTlHldwvuHXdSoOVBqrcZajz06jiFTBV9Vn3EA",
                    "merchant_fee": null,
                    "confirmation_code": null,
                    "remark": null,
                    "bank_account": null,
                    "qrcode_image_url": null,
                    "qrcode_url": null,
                    "payment_image_url": null,
                    "supplement_orders": [],
                    "supplement_orders_status": "none",
                    "payment_info": null,
                    "currency": "CNY",
                    "completed_at": null,
                    "created_at": "2024-09-25T14:33:12+08:00"
                }
                """));

        var actual = await sut.DepositAsync(merchantNo, privateKey, order);

        Assert.Equal("5dac6bd0-66ee-43e9-bf01-595a2f47f5f5", actual.ThirdPartyOrderNo);
        Assert.Equal(new Uri("https://gnngbwtf.com/orders/5dac6bd0-66ee-43e9-bf01-595a2f47f5f5?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6IjVkYWM2YmQwLTY2ZWUtNDNlOS1iZjAxLTU5NWEyZjQ3ZjVmNSJ9.UfSM2M9HqZl871I2l7KRonBa3uG4MfljeK32KB6AfymynaKhiZNPslbF7Wl0YC6wqH8G1Ad5RqH3OLHwOx-JY87aZyx64abC_qEOhkdyuuMUbYHplULegAyqSh2EpXVEJTr8WdSkqpCm9ofmGhHzBygEdJYi0K8oyDWo6tgYfxCduEro2ADQ6QKCwRh3Zl1sDVGqWWFlXqoFDT4KYSs_9I2EV1-KIyPcRmB0ncM2mZq126x5mAMS0ZJucwKIKeiY7AmeaTKC3gVP6fZHITCnMSTePl9Ut8ebALACyQIrAKnu7qxwWTlHldwvuHXdSoOVBqrcZajz06jiFTBV9Vn3EA"), 
            actual.RedirectUrl);
    }

    [Fact]
    public async Task 餘額查詢()
    {
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");
        var signer = new WFPaySigner(NullLogger<WFPaySigner>.Instance);

        var sut = new WFPayClient(httpClient, signer, NullLogger<WFPayClient>.Instance);

        var merchantNo = "merchantNo";
        var privateKey = """
    -----BEGIN RSA PRIVATE KEY-----
    MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
    zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
    GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
    JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
    q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
    EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
    e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
    Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
    6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
    CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
    IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
    zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
    ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
    NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
    DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
    jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
    kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
    oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
    ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
    WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
    Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
    dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
    kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
    HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
    8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
    -----END RSA PRIVATE KEY-----
    """;

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/balances/query")
            .Respond(HttpStatusCode.OK, new StringContent("""
                {
                    "balance": "0.0"
                }
                """));

        var actual = await sut.QueryBalanceAsync(merchantNo, privateKey);

        Assert.Equal(0, actual);
    }

    [Fact]
    public async Task 訂單查詢()
    {
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");
        var signer = new WFPaySigner(NullLogger<WFPaySigner>.Instance);

        var sut = new WFPayClient(httpClient, signer, NullLogger<WFPayClient>.Instance);

        var merchantNo = "merchantNo";
        var privateKey = """
    -----BEGIN RSA PRIVATE KEY-----
    MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
    zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
    GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
    JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
    q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
    EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
    e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
    Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
    6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
    CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
    IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
    zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
    ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
    NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
    DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
    jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
    kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
    oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
    ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
    WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
    Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
    dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
    kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
    HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
    8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
    -----END RSA PRIVATE KEY-----
    """;
        var formId = Guid.Parse("ee051ad1-05cb-4198-9aa1-e44d490a449d");

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/orders/query")
            .Respond(HttpStatusCode.Created, new StringContent("""
                {
                    "id": "5dac6bd0-66ee-43e9-bf01-595a2f47f5f5",
                    "subject": "储值",
                    "total_amount": "1000.0",
                    "notify_url": "https://jrfzmv4b-7185.asse.devtunnels.ms/deposit/callback",
                    "return_url": null,
                    "merchant_order_id": "ee051ad1-05cb-4198-9aa1-e44d490a449d",
                    "status": "init",
                    "payment_url": "https://gnngbwtf.com/orders/5dac6bd0-66ee-43e9-bf01-595a2f47f5f5?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6IjVkYWM2YmQwLTY2ZWUtNDNlOS1iZjAxLTU5NWEyZjQ3ZjVmNSJ9.UfSM2M9HqZl871I2l7KRonBa3uG4MfljeK32KB6AfymynaKhiZNPslbF7Wl0YC6wqH8G1Ad5RqH3OLHwOx-JY87aZyx64abC_qEOhkdyuuMUbYHplULegAyqSh2EpXVEJTr8WdSkqpCm9ofmGhHzBygEdJYi0K8oyDWo6tgYfxCduEro2ADQ6QKCwRh3Zl1sDVGqWWFlXqoFDT4KYSs_9I2EV1-KIyPcRmB0ncM2mZq126x5mAMS0ZJucwKIKeiY7AmeaTKC3gVP6fZHITCnMSTePl9Ut8ebALACyQIrAKnu7qxwWTlHldwvuHXdSoOVBqrcZajz06jiFTBV9Vn3EA",
                    "merchant_fee": null,
                    "confirmation_code": null,
                    "remark": null,
                    "bank_account": null,
                    "qrcode_image_url": null,
                    "qrcode_url": null,
                    "payment_image_url": null,
                    "supplement_orders": [],
                    "supplement_orders_status": "none",
                    "payment_info": null,
                    "currency": "CNY",
                    "completed_at": null,
                    "created_at": "2024-09-25T14:33:12+08:00"
                }
                """));

        var actual = await sut.QueryOrderAsync(merchantNo, privateKey, formId);

        Assert.Equal("init",actual.Status);
        Assert.Equal(1000m, actual.Amount);
        Assert.Equal("5dac6bd0-66ee-43e9-bf01-595a2f47f5f5", actual.ThirdPartyOrderNo);
    }


    [Fact]
    public async Task USDT代付()
    {
        var mockHttp = new MockHttpMessageHandler();
        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");
        var signer = new WFPaySigner(NullLogger<WFPaySigner>.Instance);

        var sut = new WFPayClient(httpClient, signer, NullLogger<WFPayClient>.Instance);

        var merchantNo = "merchantNo";
        var privateKey = """
    -----BEGIN RSA PRIVATE KEY-----
    MIIEogIBAAKCAQEAonHdMURgQR5oKoFdv0FHPQ8bML+Ou4VpCKXRhx9qF6zw/xAD
    zzcolO9RoVqfVYnRtBJnLHdICrwR9wLbnYL5JpivXW/cUzY7w0OtZ1FU3WP5rzTt
    GR6idOfUlsWO98v+cAI6v+up5zNzJnfoqVZEftcYynQSt+s6q1K5immZOhPjvRDu
    JyFcaijgs+1M7Lub1Loyb+fHhK4ANqigjJ6WIk6/xSl81vOTDj/WolxhvFd0Y8Y8
    q8F5Ug6wHyIag6PD3NF/hLbo8HedivPAT0IW3/K0YtHg1uPBaT83b/MJgi9L6CCe
    EDh+FtnNaigCAdq3InPyFcI+bRtBtkul5uJbAwIDAQABAoIBABBaD972uA22BtxI
    e2xWINqnm6ATmEWOD12ce3PzmJVbhkLWlNuV/NLJKFlqquFMZzyOIa6bLzhNaC2K
    Lxsy5OuasQRhzzbdHbTxKhJdYIKMnsShOkCuEcJc18x5hIOzGCCkSM+q5EZuqmez
    6EqrcWenNUqj1oKG/jfisuSY7qCm2NAp+H0f2kQ0jULW7/ltPAvePNIjQ2cu3Yhc
    CFG2Jjxw9sLY5pdCcEVVJDAjNP+VPZsjIgpgrKYiN8HQOycfVI768f6EHIRrDYVG
    IOiB/h92YBA7yp1eA+FDGd1q7QTZJiuWEO8KUK23m5kSIPJzXPUZqOAcknnNVgs9
    zj39AUECgYEA00ACfGUp+dPCo3CzpFuAYYKXMAWxqH4oEEIxIHyvfKRxCIJ3eKtq
    ey3ONkijetoYn9isVAJqm8HETvzBGUSjo168tnpTj3agXSTHTejW1JbHd7B7CdCa
    NnAqL/XpOxrYrBGgDHsf2u7wWeES98DQmA/kRSADwxdqn2mJgI5jDKMCgYEAxNsr
    DrjpUoUPc7r4qpwgqp66Uv/Xm9hzXEqN50t7dg8HnjMR40G7K60qb9UvpDlaw0yO
    jSNky66GdGFwN8r991i7EWUcfUXbrYHu5O3klE2SyeLj+0ND3h9YWhLS4SfOpixo
    kRLk4bWKe2L8W+rNOD1DHQAWeEsadm1cYQlc/iECgYBMOEYtV9rjVvJdQiau4Izb
    oeCm5QcKwin2Y8uR6yruD+7woRI2KbMLqqVClMbXZqWH1wemcBDDDhCkZzHMyiU4
    ps/0DwLjOmv173UxuRb6MUTa/IQIMInTpniyQUnbl34h52IV3r7JoXh7FWhgl+Mr
    WmCESF7THznLNqr0BWvbfQKBgE4Y5SwiD+68+zReqOiGqC8IacMj3NXnOV0T9tgc
    Cg78R3lrqV0PZsIdIQjc+tmcJ3XzmVpFAGgOBSmGvo5j6f2HJi/wfSz12gEr2yrX
    dief3uxZJzk3pmUo6lRRw3ev4zvpwlJLvKIO87Gfw55zuJbnNwj6T+/JW+Y/3LZi
    kJIBAoGAI/GjrKmex4w8aHNT+UtRfCCoh1chKjdBFuuz3QNSLH+j/xIx6XzgMh50
    HwUENRf6FwbHAzATo4ht3FyzEIwSbchTpl5YKXQzMSvHU9OS0axFJhmZ0Z11GV9S
    8J1lA6/OgITirkJrSwdBcbYQmkmF3I8CdZpAzVC1tj76dziUIvI=
    -----END RSA PRIVATE KEY-----
    """;

        var usdtOrder = new UsdtWithdrawOrder
        {
            Amount = 1,
            FormId = Guid.NewGuid(),
            BlockchainAddress = "test",
            CallbackUrl = new Uri("https://jrfzmv4b-7185.asse.devtunnels.ms/withdraw/callback")
        };

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/orders/usdt_payment_transfer")
            .Respond(HttpStatusCode.Forbidden, new StringContent("""
                {"message":"blockchain_address 区块链地址错误","error_key":"parameter_invalid"}
                """));

        _ = await Assert.ThrowsAsync<ApiFailException>(async () => await sut.WithdrawUsdtAsync(merchantNo, privateKey, usdtOrder));
    }
}