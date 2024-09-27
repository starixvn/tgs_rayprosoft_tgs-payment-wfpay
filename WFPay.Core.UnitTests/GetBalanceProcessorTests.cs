using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.Payment.WFPay;
using Tgs.Payment.WFPay.MessageQueue;
using Tgs.Payment.Protos;

namespace WFPay.Core.UnitTests;

public class GetBalanceProcessorTests
{
    [Fact]
    public async Task GetBalanceProcessor_取得商戶餘額()
    {
        // Arrange
        var mockHttp = new MockHttpMessageHandler();

        var httpClient = mockHttp.ToHttpClient();
        httpClient.BaseAddress = new Uri("https://localhost");

        var fakePayClient = new WFPayClient(
            httpClient,
            new WFPaySigner(NullLogger<WFPaySigner>.Instance),
            NullLogger<WFPayClient>.Instance);

        var sut = new GetBalanceProcessor(fakePayClient);

        var request = new PaymentBalanceRequest
        {
            BrandPaymentSettings = """
            {
                "merchantNo": "merchantNo",
                "privateKey": "LS0tLS1CRUdJTiBSU0EgUFJJVkFURSBLRVktLS0tLQ0KTUlJRW9nSUJBQUtDQVFFQW9uSGRNVVJnUVI1b0tvRmR2MEZIUFE4Yk1MK091NFZwQ0tYUmh4OXFGNnp3L3hBRA0Kenpjb2xPOVJvVnFmVlluUnRCSm5MSGRJQ3J3Ujl3TGJuWUw1SnBpdlhXL2NVelk3dzBPdFoxRlUzV1A1cnpUdA0KR1I2aWRPZlVsc1dPOTh2K2NBSTZ2K3VwNXpOekpuZm9xVlpFZnRjWXluUVN0K3M2cTFLNWltbVpPaFBqdlJEdQ0KSnlGY2FpamdzKzFNN0x1YjFMb3liK2ZIaEs0QU5xaWdqSjZXSWs2L3hTbDgxdk9URGovV29seGh2RmQwWThZOA0KcThGNVVnNndIeUlhZzZQRDNORi9oTGJvOEhlZGl2UEFUMElXMy9LMFl0SGcxdVBCYVQ4M2IvTUpnaTlMNkNDZQ0KRURoK0Z0bk5haWdDQWRxM0luUHlGY0krYlJ0QnRrdWw1dUpiQXdJREFRQUJBb0lCQUJCYUQ5NzJ1QTIyQnR4SQ0KZTJ4V0lOcW5tNkFUbUVXT0QxMmNlM1B6bUpWYmhrTFdsTnVWL05MSktGbHFxdUZNWnp5T0lhNmJMemhOYUMySw0KTHhzeTVPdWFzUVJoenpiZEhiVHhLaEpkWUlLTW5zU2hPa0N1RWNKYzE4eDVoSU96R0NDa1NNK3E1RVp1cW1leg0KNkVxcmNXZW5OVXFqMW9LRy9qZmlzdVNZN3FDbTJOQXArSDBmMmtRMGpVTFc3L2x0UEF2ZVBOSWpRMmN1M1loYw0KQ0ZHMkpqeHc5c0xZNXBkQ2NFVlZKREFqTlArVlBac2pJZ3BncktZaU44SFFPeWNmVkk3NjhmNkVISVJyRFlWRw0KSU9pQi9oOTJZQkE3eXAxZUErRkRHZDFxN1FUWkppdVdFTzhLVUsyM201a1NJUEp6WFBVWnFPQWNrbm5OVmdzOQ0KemozOUFVRUNnWUVBMDBBQ2ZHVXArZFBDbzNDenBGdUFZWUtYTUFXeHFING9FRUl4SUh5dmZLUnhDSUozZUt0cQ0KZXkzT05raWpldG9Zbjlpc1ZBSnFtOEhFVHZ6QkdVU2pvMTY4dG5wVGozYWdYU1RIVGVqVzFKYkhkN0I3Q2RDYQ0KTm5BcUwvWHBPeHJZckJHZ0RIc2YydTd3V2VFUzk4RFFtQS9rUlNBRHd4ZHFuMm1KZ0k1akRLTUNnWUVBeE5zcg0KRHJqcFVvVVBjN3I0cXB3Z3FwNjZVdi9YbTloelhFcU41MHQ3ZGc4SG5qTVI0MEc3SzYwcWI5VXZwRGxhdzB5Tw0KalNOa3k2NkdkR0Z3TjhyOTkxaTdFV1VjZlVYYnJZSHU1TzNrbEUyU3llTGorME5EM2g5WVdoTFM0U2ZPcGl4bw0Ka1JMazRiV0tlMkw4VytyTk9EMURIUUFXZUVzYWRtMWNZUWxjL2lFQ2dZQk1PRVl0VjlyalZ2SmRRaWF1NEl6Yg0Kb2VDbTVRY0t3aW4yWTh1UjZ5cnVEKzd3b1JJMktiTUxxcVZDbE1iWFpxV0gxd2VtY0JERERoQ2taekhNeWlVNA0KcHMvMER3TGpPbXYxNzNVeHVSYjZNVVRhL0lRSU1JblRwbml5UVVuYmwzNGg1MklWM3I3Sm9YaDdGV2hnbCtNcg0KV21DRVNGN1RIem5MTnFyMEJXdmJmUUtCZ0U0WTVTd2lEKzY4K3pSZXFPaUdxQzhJYWNNajNOWG5PVjBUOXRnYw0KQ2c3OFIzbHJxVjBQWnNJZElRamMrdG1jSjNYem1WcEZBR2dPQlNtR3ZvNWo2ZjJISmkvd2ZTejEyZ0VyMnlyWA0KZGllZjN1eFpKemszcG1VbzZsUlJ3M2V2NHp2cHdsSkx2S0lPODdHZnc1NXp1SmJuTndqNlQrL0pXK1kvM0xaaQ0Ka0pJQkFvR0FJL0dqckttZXg0dzhhSE5UK1V0UmZDQ29oMWNoS2pkQkZ1dXozUU5TTEgrai94SXg2WHpnTWg1MA0KSHdVRU5SZjZGd2JIQXpBVG80aHQzRnl6RUl3U2JjaFRwbDVZS1hRek1TdkhVOU9TMGF4RkpobVowWjExR1Y5Uw0KOEoxbEE2L09nSVRpcmtKclN3ZEJjYllRbWttRjNJOENkWnBBelZDMXRqNzZkemlVSXZJPQ0KLS0tLS1FTkQgUlNBIFBSSVZBVEUgS0VZLS0tLS0="
            }
            """
        };

        _ = mockHttp.Expect(HttpMethod.Post, "https://localhost/merchant_api/v1/balances/query")
            .Respond(HttpStatusCode.OK, new StringContent("""
                {
                    "balance": "0.0"
                }
                """));

        // Act
        var actual = await sut.HandleAsync("test", request, null);

        // Assert
        Assert.Equal(0M, actual.Balance.ToDecimal());
    }
}