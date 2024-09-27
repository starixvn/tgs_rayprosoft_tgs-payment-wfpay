using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Tgs.Payment.WFPay.Client;
using Tgs.Payment.Clients;
using Xunit.Abstractions;

namespace Tgs.Payment.WFPay.IntegrationTests;

public class DepositTests(ITestOutputHelper testOutputHelper)
{
    private readonly ITestOutputHelper _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));

    [Fact]
    public async Task 代收回調_訂單狀態成功()
    {
        var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
        var fakePaymentClient = Substitute.For<IPaymentClient>();

        await using var web = WFPayTestHost.Factory
            .WithWebHostBuilder(hostBuilder => hostBuilder
                .ConfigureLogging(logging => logging.AddXUnit(_testOutputHelper))
                .ConfigureServices(services => services
                    .AddSingleton(fakePaymentServiceClient)
                    .AddSingleton(fakePaymentClient)));

        var httpClient = web.CreateClient();

        var formId = Guid.Parse("b002ca16-3d4d-485f-ab00-20829fc05718");
        var brandId = Guid.NewGuid();

        var request = new Dictionary<string, string>
        {
            ["data"] = """
                {"order":{"id":"c89127ee-5e01-4c92-942b-ed8a32e9b3b3","subject":"储值","total_amount":"1000.0","notify_url":"https://jrfzmv4b-7185.asse.devtunnels.ms/deposit/callback","return_url":null,"merchant_order_id":"b002ca16-3d4d-485f-ab00-20829fc05718","status":"completed","payment_url":"https://gnngbwtf.com/orders/c89127ee-5e01-4c92-942b-ed8a32e9b3b3?token=eyJhbGciOiJSUzI1NiJ9.eyJvcmRlcl9pZCI6ImM4OTEyN2VlLTVlMDEtNGM5Mi05NDJiLWVkOGEzMmU5YjNiMyJ9.YHg6ly8GG0PJDsUvxWS-4uFeBY_6tFCYjyBw4gNJPZagLd6XiO8oX5lsV7F1mTwuWZAkgBr0mWCt4mvr8SENYNlP8-fA9m6ivpALWwRBpjg_bojLaDaGTkgNjkq7wSrBy41qXvCpXaiH93stYJwwI4Rfhgs8O-01a36eaYuJQ5UR_FgdTKdP4hk9LA4fmje0wptkJLRymNoSH9ooDhSsjrUYo_5wZcDC7NilOQ2LPOzfL0rMQZVDK5q2s06BEY-v5TcgLl4UGrzgOOSYjdDaf90if9lirOuTSTpaW0w1LPOnFrbQUlvZshK_RS1hapFUdOXo5OZoDAo_gRK5WR5g2Q","merchant_fee":"43.0","confirmation_code":"34939","remark":null,"bank_account":null,"qrcode_image_url":null,"qrcode_url":null,"payment_image_url":null,"supplement_orders":[],"supplement_orders_status":"none","payment_info":{"account_name":"18137030255","surname":"孟","name":"孟"},"currency":"CNY","completed_at":"2024-09-26T11:08:06+08:00","created_at":"2024-09-26T10:10:45+08:00"},"notify_type":"trade_completed"}
                """,
            ["signature"] = "w3AB8LTIJcmP9rKcDJmGDrr5SFIBzDkSMjeg33B1jOMraDwn1Rq+e+LC4QDWbORYYBYO55WAB5Uwv8lcyDR2NPWEPsiS+qL/BPEIxH31+/KTP+q0xWpWgcDjPyX/VOYPsO6oFzEbL/R0wlD+rTqT2edKiY2W9knMF1tvOZkAvQFPw0xqEUkzjcX/iV/UjtVSO5P37QJtKp4ppSQjbZtUQtpXKeyG8SdhnqLgS+CkVDPtYCdcvAzxhaQcXyeibzRv0pZP/Iahv3DzcEFbTraHaiI/mo7JLV5sPBTDuU+ydLs1JIdSIav5bSJEacdyFbngtbaOx0y7lCF2XDwF9x6Opw=="
        };

        var content = new FormUrlEncodedContent(request);

        var applyInfo = new WFPayDepositApplyInfo(
            new WFPayDepositSettings(
                "amuse888",
                "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQ0KTUlJRW9nSUJBQUtDQVFFQW9uSGRNVVJnUVI1b0tvRmR2MEZIUFE4Yk1MK091NFZwQ0tYUmh4OXFGNnp3L3hBRA0Kenpjb2xPOVJvVnFmVlluUnRCSm5MSGRJQ3J3Ujl3TGJuWUw1SnBpdlhXL2NVelk3dzBPdFoxRlUzV1A1cnpUdA0KR1I2aWRPZlVsc1dPOTh2K2NBSTZ2K3VwNXpOekpuZm9xVlpFZnRjWXluUVN0K3M2cTFLNWltbVpPaFBqdlJEdQ0KSnlGY2FpamdzKzFNN0x1YjFMb3liK2ZIaEs0QU5xaWdqSjZXSWs2L3hTbDgxdk9URGovV29seGh2RmQwWThZOA0KcThGNVVnNndIeUlhZzZQRDNORi9oTGJvOEhlZGl2UEFUMElXMy9LMFl0SGcxdVBCYVQ4M2IvTUpnaTlMNkNDZQ0KRURoK0Z0bk5haWdDQWRxM0luUHlGY0krYlJ0QnRrdWw1dUpiQXdJREFRQUJBb0lCQUJCYUQ5NzJ1QTIyQnR4SQ0KZTJ4V0lOcW5tNkFUbUVXT0QxMmNlM1B6bUpWYmhrTFdsTnVWL05MSktGbHFxdUZNWnp5T0lhNmJMemhOYUMySw0KTHhzeTVPdWFzUVJoenpiZEhiVHhLaEpkWUlLTW5zU2hPa0N1RWNKYzE4eDVoSU96R0NDa1NNK3E1RVp1cW1leg0KNkVxcmNXZW5OVXFqMW9LRy9qZmlzdVNZN3FDbTJOQXArSDBmMmtRMGpVTFc3L2x0UEF2ZVBOSWpRMmN1M1loYw0KQ0ZHMkpqeHc5c0xZNXBkQ2NFVlZKREFqTlArVlBac2pJZ3BncktZaU44SFFPeWNmVkk3NjhmNkVISVJyRFlWRw0KSU9pQi9oOTJZQkE3eXAxZUErRkRHZDFxN1FUWkppdVdFTzhLVUsyM201a1NJUEp6WFBVWnFPQWNrbm5OVmdzOQ0KemozOUFVRUNnWUVBMDBBQ2ZHVXArZFBDbzNDenBGdUFZWUtYTUFXeHFING9FRUl4SUh5dmZLUnhDSUozZUt0cQ0KZXkzT05raWpldG9Zbjlpc1ZBSnFtOEhFVHZ6QkdVU2pvMTY4dG5wVGozYWdYU1RIVGVqVzFKYkhkN0I3Q2RDYQ0KTm5BcUwvWHBPeHJZckJHZ0RIc2YydTd3V2VFUzk4RFFtQS9rUlNBRHd4ZHFuMm1KZ0k1akRLTUNnWUVBeE5zcg0KRHJqcFVvVVBjN3I0cXB3Z3FwNjZVdi9YbTloelhFcU41MHQ3ZGc4SG5qTVI0MEc3SzYwcWI5VXZwRGxhdzB5Tw0KalNOa3k2NkdkR0Z3TjhyOTkxaTdFV1VjZlVYYnJZSHU1TzNrbEUyU3llTGorME5EM2g5WVdoTFM0U2ZPcGl4bw0Ka1JMazRiV0tlMkw4VytyTk9EMURIUUFXZUVzYWRtMWNZUWxjL2lFQ2dZQk1PRVl0VjlyalZ2SmRRaWF1NEl6Yg0Kb2VDbTVRY0t3aW4yWTh1UjZ5cnVEKzd3b1JJMktiTUxxcVZDbE1iWFpxV0gxd2VtY0JERERoQ2taekhNeWlVNA0KcHMvMER3TGpPbXYxNzNVeHVSYjZNVVRhL0lRSU1JblRwbml5UVVuYmwzNGg1MklWM3I3Sm9YaDdGV2hnbCtNcg0KV21DRVNGN1RIem5MTnFyMEJXdmJmUUtCZ0U0WTVTd2lEKzY4K3pSZXFPaUdxQzhJYWNNajNOWG5PVjBUOXRnYw0KQ2c3OFIzbHJxVjBQWnNJZElRamMrdG1jSjNYem1WcEZBR2dPQlNtR3ZvNWo2ZjJISmkvd2ZTejEyZ0VyMnlyWA0KZGllZjN1eFpKemszcG1VbzZsUlJ3M2V2NHp2cHdsSkx2S0lPODdHZnc1NXp1SmJuTndqNlQrL0pXK1kvM0xaaQ0Ka0pJQkFvR0FJL0dqckttZXg0dzhhSE5UK1V0UmZDQ29oMWNoS2pkQkZ1dXozUU5TTEgrai94SXg2WHpnTWg1MA0KSHdVRU5SZjZGd2JIQXpBVG80aHQzRnl6RUl3U2JjaFRwbDVZS1hRek1TdkhVOU9TMGF4RkpobVowWjExR1Y5Uw0KOEoxbEE2L09nSVRpcmtKclN3ZEJjYllRbWttRjNJOENkWnBBelZDMXRqNzZkemlVSXZJPQ0KLS0tLS1FTkQgUlNBIFBSSVZBVEUgS0VZLS0tLS0=",
                "LS0tLS1CRUdJTiBQVUJMSUMgS0VZLS0tLS0NCk1JSUJJakFOQmdrcWhraUc5dzBCQVFFRkFBT0NBUThBTUlJQkNnS0NBUUVBNlNCSDNnSCtBMjc1UkhvcU1sYy8NCkM5MzJYS0RQYmhhK2drSmhkVzdqeXB5NXB6ekU3eTN5KzF4RDVtOHRBK3ZwUFhNVDJmbDYyUjRIK3dyM2FpYXUNClF1cFhxMjR5WkFxNWIzRDV4OGxuUlNvSGNTeW1EV2VTcjBlL2FuWmNJNklscWJYZm8wNVRhNGo0K0Y3VGhBUjANClRNSzNtcnZUUVB2QVI5T2hpSE9rWS9KS1NEQW8yL3lFUlpLdFBWV2ZIM1U1WFBpVmNYb0kzcHNHVXBvR3kzREINCmNVR3Z1SDFBcnZGSHJsOUpsbGZMQWNRUTViLzFtK1ZENVBUNVBDY1NBMGV4ZzljeHNyUytvaWg0dTE4T21IaS8NCmFWYW1ncjlCNDRhOU5LWGo4Ykp1UzlSaFBDY1YzNXh3M2g5WCtUS2tCcFZnRDl5aHFsK3FrZXlyUXg3OVdCRmoNCmhRSURBUUFCDQotLS0tLUVORCBQVUJMSUMgS0VZLS0tLS0=",
                formId,
                "alipay"),
             new WFPayDepositResponse(
                1000m,
                formId,
                new Uri("https://fukui.exlepoa.com/api/payment_direct/redirect/HDRB4529214901780238336"),
                "c89127ee-5e01-4c92-942b-ed8a32e9b3b3"));

        _ = fakePaymentServiceClient.GetDepositFormAsync(Arg.Is(formId), Arg.Any<CancellationToken>())
            .Returns(new DepositFormInfo
            {
                Id = formId,
                BrandId = brandId,
                Status = PaymentFlowState.WaitPayment,
                Amount = 1000m,
                ThirdPartyOrderNo = "c89127ee-5e01-4c92-942b-ed8a32e9b3b3",
                ApplyInfo = JsonSerializer.Serialize(applyInfo, WFPayUtility.SerializerOptions)
            });

        var response = await httpClient.PostAsync("/deposit/callback", content);

        var actual = await response.Content.ReadAsStringAsync();

        Assert.Equal("success", actual);

        _ = fakePaymentServiceClient.Received(1)
            .SubmitDepositFormStatusAsync(
                Arg.Is(formId),
                paymentReturn: Arg.Any<string>(),
                cancellationToken: Arg.Any<CancellationToken>());

        _ = fakePaymentClient.Received(1)
            .ApproveDepositAsync(
                Arg.Is(formId),
                Arg.Is(brandId),
                Arg.Is(1000m),
                Arg.Any<CancellationToken>());
    }
}