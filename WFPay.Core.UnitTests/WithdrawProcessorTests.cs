using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using RichardSzalay.MockHttp;
using Tgs.Payment.WFPay;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.WFPay.MessageQueue;
using Tgs.Payment.Clients;
using Tgs.Payment.Protos;

namespace WFPay.Core.UnitTests;

public class WithdrawProcessorTests
{
    [Fact]
    public async Task 代付_廠商範例()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");

        var fakePayClient = new WFPayClient(
            httpClient,
            new WFPaySigner(NullLogger<WFPaySigner>.Instance),
            NullLogger<WFPayClient>.Instance);

        var fakePaymentClient = Substitute.For<IPaymentClient>();
        var options = new WFPayOptions
        {
            CallbackBaseUrl = new Uri("https://localhost"),
            PaymentApiUrl = new Uri("https://localhost"),
            PaymentServiceBaseUrl = new Uri("http://localhost:5541")
        };

        var sut = new WithdrawProcessor(
            fakePayClient,
            new OptionsWrapper<WFPayOptions>(options),
            fakePaymentClient,
            NullLogger<WithdrawProcessor>.Instance);

        var formId = Guid.Parse("508c48e8b7f5df2ce9bf440747bdb7ef");        

        var applySettings = new UsdtWithdrawSettings(
            "amuse888",
            "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQ0KTUlJRW9nSUJBQUtDQVFFQW9uSGRNVVJnUVI1b0tvRmR2MEZIUFE4Yk1MK091NFZwQ0tYUmh4OXFGNnp3L3hBRA0Kenpjb2xPOVJvVnFmVlluUnRCSm5MSGRJQ3J3Ujl3TGJuWUw1SnBpdlhXL2NVelk3dzBPdFoxRlUzV1A1cnpUdA0KR1I2aWRPZlVsc1dPOTh2K2NBSTZ2K3VwNXpOekpuZm9xVlpFZnRjWXluUVN0K3M2cTFLNWltbVpPaFBqdlJEdQ0KSnlGY2FpamdzKzFNN0x1YjFMb3liK2ZIaEs0QU5xaWdqSjZXSWs2L3hTbDgxdk9URGovV29seGh2RmQwWThZOA0KcThGNVVnNndIeUlhZzZQRDNORi9oTGJvOEhlZGl2UEFUMElXMy9LMFl0SGcxdVBCYVQ4M2IvTUpnaTlMNkNDZQ0KRURoK0Z0bk5haWdDQWRxM0luUHlGY0krYlJ0QnRrdWw1dUpiQXdJREFRQUJBb0lCQUJCYUQ5NzJ1QTIyQnR4SQ0KZTJ4V0lOcW5tNkFUbUVXT0QxMmNlM1B6bUpWYmhrTFdsTnVWL05MSktGbHFxdUZNWnp5T0lhNmJMemhOYUMySw0KTHhzeTVPdWFzUVJoenpiZEhiVHhLaEpkWUlLTW5zU2hPa0N1RWNKYzE4eDVoSU96R0NDa1NNK3E1RVp1cW1leg0KNkVxcmNXZW5OVXFqMW9LRy9qZmlzdVNZN3FDbTJOQXArSDBmMmtRMGpVTFc3L2x0UEF2ZVBOSWpRMmN1M1loYw0KQ0ZHMkpqeHc5c0xZNXBkQ2NFVlZKREFqTlArVlBac2pJZ3BncktZaU44SFFPeWNmVkk3NjhmNkVISVJyRFlWRw0KSU9pQi9oOTJZQkE3eXAxZUErRkRHZDFxN1FUWkppdVdFTzhLVUsyM201a1NJUEp6WFBVWnFPQWNrbm5OVmdzOQ0KemozOUFVRUNnWUVBMDBBQ2ZHVXArZFBDbzNDenBGdUFZWUtYTUFXeHFING9FRUl4SUh5dmZLUnhDSUozZUt0cQ0KZXkzT05raWpldG9Zbjlpc1ZBSnFtOEhFVHZ6QkdVU2pvMTY4dG5wVGozYWdYU1RIVGVqVzFKYkhkN0I3Q2RDYQ0KTm5BcUwvWHBPeHJZckJHZ0RIc2YydTd3V2VFUzk4RFFtQS9rUlNBRHd4ZHFuMm1KZ0k1akRLTUNnWUVBeE5zcg0KRHJqcFVvVVBjN3I0cXB3Z3FwNjZVdi9YbTloelhFcU41MHQ3ZGc4SG5qTVI0MEc3SzYwcWI5VXZwRGxhdzB5Tw0KalNOa3k2NkdkR0Z3TjhyOTkxaTdFV1VjZlVYYnJZSHU1TzNrbEUyU3llTGorME5EM2g5WVdoTFM0U2ZPcGl4bw0Ka1JMazRiV0tlMkw4VytyTk9EMURIUUFXZUVzYWRtMWNZUWxjL2lFQ2dZQk1PRVl0VjlyalZ2SmRRaWF1NEl6Yg0Kb2VDbTVRY0t3aW4yWTh1UjZ5cnVEKzd3b1JJMktiTUxxcVZDbE1iWFpxV0gxd2VtY0JERERoQ2taekhNeWlVNA0KcHMvMER3TGpPbXYxNzNVeHVSYjZNVVRhL0lRSU1JblRwbml5UVVuYmwzNGg1MklWM3I3Sm9YaDdGV2hnbCtNcg0KV21DRVNGN1RIem5MTnFyMEJXdmJmUUtCZ0U0WTVTd2lEKzY4K3pSZXFPaUdxQzhJYWNNajNOWG5PVjBUOXRnYw0KQ2c3OFIzbHJxVjBQWnNJZElRamMrdG1jSjNYem1WcEZBR2dPQlNtR3ZvNWo2ZjJISmkvd2ZTejEyZ0VyMnlyWA0KZGllZjN1eFpKemszcG1VbzZsUlJ3M2V2NHp2cHdsSkx2S0lPODdHZnc1NXp1SmJuTndqNlQrL0pXK1kvM0xaaQ0Ka0pJQkFvR0FJL0dqckttZXg0dzhhSE5UK1V0UmZDQ29oMWNoS2pkQkZ1dXozUU5TTEgrai94SXg2WHpnTWg1MA0KSHdVRU5SZjZGd2JIQXpBVG80aHQzRnl6RUl3U2JjaFRwbDVZS1hRek1TdkhVOU9TMGF4RkpobVowWjExR1Y5Uw0KOEoxbEE2L09nSVRpcmtKclN3ZEJjYllRbWttRjNJOENkWnBBelZDMXRqNzZkemlVSXZJPQ0KLS0tLS1FTkQgUlNBIFBSSVZBVEUgS0VZLS0tLS0=",
            "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0NCk1JSUJJakFOQmdrcWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBNlNCSDNnSCtBMjc1UkhvcU1sYy8NCkM5MzJYS0RQYmhhK2drSmhkVzdqeXB5NXB6ekU3eTN5KzF4RDVtOHRBK3ZwUFhNVDJmbDYyUjRIK3dyM2FpYXUNClF1cFhxMjR5WkFxNWIzRDV4OGxuUlNvSGNTeW1EV2VTcjBlL2FuWmNJNklscWJYZm8wNVRhNGo0K0Y3VGhBUjANClRNSzNtcnZUUVB2QVI5T2hpSE9rWS9KS1NEQW8yL3lFUlpLdFBWV2ZIM1U1WFBpVmNYb0kzcHNHVXBvR3kzREINCmNVR3Z1SDFBcnZGSHJsOUpsbGZMQWNRUTViLzFtK1ZENVBUNVBDY1NBMGV4ZzljeHNyUytvaWg0dTE4T21IaS8NCmFWYW1ncjlCNDRhOU5LWGo4Ykp1UzlSaFBDY1YzNXh3M2g5WCtUS2tCcFZnRDl5aHFsK3FrZXlyUXg3OVdCRmoNCmhRSURBUUFCDQotLS0tLUVORCBQVUJMSUMgS0VZLS0tLS0=",
            formId,
            "TU1DhZdh6kztjzRmb7QDhcgWirYm4CSW3U");

        var request = new WithdrawRequest
        {
            Amount = "1",
            ApplyInfo = JsonSerializer.Serialize(applySettings, WFPayUtility.SerializerOptions)
        };

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/orders/usdt_payment_transfer")
            .Respond(HttpStatusCode.Created, new StringContent("""
                {
                  "id": "b4225bbb-8610-45e1-b948-6cd8c092af65",
                  "subject": "USDT 代付",
                  "total_amount": "1.0",
                  "notify_url": "http://example/webhooks/webhook_test",
                  "merchant_order_id": "508c48e8b7f5df2ce9bf440747bdb7ef",
                  "status": "init",
                  "completed_at": null,
                  "merchant_fee": null,
                  "created_at": "2016-12-12T23:12:27+08:00"
                }
                """));

        // Act
        var actual = await sut.HandleAsync(
            "test",
            request,
            null);

        var applyInfo = JsonSerializer.Deserialize<WFPayWithdrawApplyInfo>(actual.Result, WFPayUtility.SerializerOptions);

        // Assert
        _ = fakePaymentClient.Received(1)
            .SaveWithdrawFormThirdPartyOrderNoAsync(
                Arg.Is(applySettings.FormId),
                Arg.Is("b4225bbb-8610-45e1-b948-6cd8c092af65"));
        Assert.NotNull(applyInfo);
        Assert.Equal(applySettings, applyInfo.Settings);
    }
}