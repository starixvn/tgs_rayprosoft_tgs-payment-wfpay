using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using RichardSzalay.MockHttp;
using Tgs.Payment.WFPay;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.WFPay.MessageQueue;
using Tgs.Payment.Protos;

namespace WFPay.Core.UnitTests;

public class DepositProcessorTests
{
    [Fact]
    public async Task 儲值要求處理()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");

        var fakePayClient = new WFPayClient(
            httpClient,
            new WFPaySigner(NullLogger<WFPaySigner>.Instance),
            NullLogger<WFPayClient>.Instance);

        var options = new WFPayOptions
        {
            CallbackBaseUrl = new Uri("https://localhost"),
            PaymentApiUrl = new Uri("https://localhost"),
            PaymentServiceBaseUrl = new Uri("http://localhost:5541")
        };
        var formId = Guid.Parse("ee051ad1-05cb-4198-9aa1-e44d490a449d");
        var merchantNo = "amuse888";
        var privateKey = "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQ0KTUlJRW9nSUJBQUtDQVFFQW9uSGRNVVJnUVI1b0tvRmR2MEZIUFE4Yk1MK091NFZwQ0tYUmh4OXFGNnp3L3hBRA0Kenpjb2xPOVJvVnFmVlluUnRCSm5MSGRJQ3J3Ujl3TGJuWUw1SnBpdlhXL2NVelk3dzBPdFoxRlUzV1A1cnpUdA0KR1I2aWRPZlVsc1dPOTh2K2NBSTZ2K3VwNXpOekpuZm9xVlpFZnRjWXluUVN0K3M2cTFLNWltbVpPaFBqdlJEdQ0KSnlGY2FpamdzKzFNN0x1YjFMb3liK2ZIaEs0QU5xaWdqSjZXSWs2L3hTbDgxdk9URGovV29seGh2RmQwWThZOA0KcThGNVVnNndIeUlhZzZQRDNORi9oTGJvOEhlZGl2UEFUMElXMy9LMFl0SGcxdVBCYVQ4M2IvTUpnaTlMNkNDZQ0KRURoK0Z0bk5haWdDQWRxM0luUHlGY0krYlJ0QnRrdWw1dUpiQXdJREFRQUJBb0lCQUJCYUQ5NzJ1QTIyQnR4SQ0KZTJ4V0lOcW5tNkFUbUVXT0QxMmNlM1B6bUpWYmhrTFdsTnVWL05MSktGbHFxdUZNWnp5T0lhNmJMemhOYUMySw0KTHhzeTVPdWFzUVJoenpiZEhiVHhLaEpkWUlLTW5zU2hPa0N1RWNKYzE4eDVoSU96R0NDa1NNK3E1RVp1cW1leg0KNkVxcmNXZW5OVXFqMW9LRy9qZmlzdVNZN3FDbTJOQXArSDBmMmtRMGpVTFc3L2x0UEF2ZVBOSWpRMmN1M1loYw0KQ0ZHMkpqeHc5c0xZNXBkQ2NFVlZKREFqTlArVlBac2pJZ3BncktZaU44SFFPeWNmVkk3NjhmNkVISVJyRFlWRw0KSU9pQi9oOTJZQkE3eXAxZUErRkRHZDFxN1FUWkppdVdFTzhLVUsyM201a1NJUEp6WFBVWnFPQWNrbm5OVmdzOQ0KemozOUFVRUNnWUVBMDBBQ2ZHVXArZFBDbzNDenBGdUFZWUtYTUFXeHFING9FRUl4SUh5dmZLUnhDSUozZUt0cQ0KZXkzT05raWpldG9Zbjlpc1ZBSnFtOEhFVHZ6QkdVU2pvMTY4dG5wVGozYWdYU1RIVGVqVzFKYkhkN0I3Q2RDYQ0KTm5BcUwvWHBPeHJZckJHZ0RIc2YydTd3V2VFUzk4RFFtQS9rUlNBRHd4ZHFuMm1KZ0k1akRLTUNnWUVBeE5zcg0KRHJqcFVvVVBjN3I0cXB3Z3FwNjZVdi9YbTloelhFcU41MHQ3ZGc4SG5qTVI0MEc3SzYwcWI5VXZwRGxhdzB5Tw0KalNOa3k2NkdkR0Z3TjhyOTkxaTdFV1VjZlVYYnJZSHU1TzNrbEUyU3llTGorME5EM2g5WVdoTFM0U2ZPcGl4bw0Ka1JMazRiV0tlMkw4VytyTk9EMURIUUFXZUVzYWRtMWNZUWxjL2lFQ2dZQk1PRVl0VjlyalZ2SmRRaWF1NEl6Yg0Kb2VDbTVRY0t3aW4yWTh1UjZ5cnVEKzd3b1JJMktiTUxxcVZDbE1iWFpxV0gxd2VtY0JERERoQ2taekhNeWlVNA0KcHMvMER3TGpPbXYxNzNVeHVSYjZNVVRhL0lRSU1JblRwbml5UVVuYmwzNGg1MklWM3I3Sm9YaDdGV2hnbCtNcg0KV21DRVNGN1RIem5MTnFyMEJXdmJmUUtCZ0U0WTVTd2lEKzY4K3pSZXFPaUdxQzhJYWNNajNOWG5PVjBUOXRnYw0KQ2c3OFIzbHJxVjBQWnNJZElRamMrdG1jSjNYem1WcEZBR2dPQlNtR3ZvNWo2ZjJISmkvd2ZTejEyZ0VyMnlyWA0KZGllZjN1eFpKemszcG1VbzZsUlJ3M2V2NHp2cHdsSkx2S0lPODdHZnc1NXp1SmJuTndqNlQrL0pXK1kvM0xaaQ0Ka0pJQkFvR0FJL0dqckttZXg0dzhhSE5UK1V0UmZDQ29oMWNoS2pkQkZ1dXozUU5TTEgrai94SXg2WHpnTWg1MA0KSHdVRU5SZjZGd2JIQXpBVG80aHQzRnl6RUl3U2JjaFRwbDVZS1hRek1TdkhVOU9TMGF4RkpobVowWjExR1Y5Uw0KOEoxbEE2L09nSVRpcmtKclN3ZEJjYllRbWttRjNJOENkWnBBelZDMXRqNzZkemlVSXZJPQ0KLS0tLS1FTkQgUlNBIFBSSVZBVEUgS0VZLS0tLS0=";
        var publicKey = "";

        var sut = new DepositProcessor(
            fakePayClient,
            new OptionsWrapper<WFPayOptions>(options),
            NullLogger<DepositProcessor>.Instance);

        var applySettings = new WFPayDepositSettings(
            merchantNo,
            privateKey,
            publicKey,
            formId,
            "alipay");

        var request = new DepositRequest
        {
            Amount = "1000",
            ApplyInfo = JsonSerializer.Serialize(applySettings, WFPayUtility.SerializerOptions)
        };

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

        // Act
        var actual = await sut.HandleAsync(
            "test",
            request,
            null);

        var applyInfo = JsonSerializer.Deserialize<WFPayDepositApplyInfo>(actual.Result, WFPayUtility.SerializerOptions);

        Assert.NotNull(applyInfo);
        Assert.Equal(applySettings, applyInfo.Settings);
        Assert.Equal(
            new WFPayDepositResponse(
                1000.0m,
                applySettings.FormId,
                new Uri("https://gnngbwtf.com/orders/5dac6bd0-66ee-43e9-bf01-595a2f47f5f5?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6IjVkYWM2YmQwLTY2ZWUtNDNlOS1iZjAxLTU5NWEyZjQ3ZjVmNSJ9.UfSM2M9HqZl871I2l7KRonBa3uG4MfljeK32KB6AfymynaKhiZNPslbF7Wl0YC6wqH8G1Ad5RqH3OLHwOx-JY87aZyx64abC_qEOhkdyuuMUbYHplULegAyqSh2EpXVEJTr8WdSkqpCm9ofmGhHzBygEdJYi0K8oyDWo6tgYfxCduEro2ADQ6QKCwRh3Zl1sDVGqWWFlXqoFDT4KYSs_9I2EV1-KIyPcRmB0ncM2mZq126x5mAMS0ZJucwKIKeiY7AmeaTKC3gVP6fZHITCnMSTePl9Ut8ebALACyQIrAKnu7qxwWTlHldwvuHXdSoOVBqrcZajz06jiFTBV9Vn3EA"),
                "5dac6bd0-66ee-43e9-bf01-595a2f47f5f5"),
            applyInfo.Response);
    }
}