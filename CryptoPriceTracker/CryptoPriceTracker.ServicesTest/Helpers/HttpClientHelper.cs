namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class HttpClientHelper
{
    public static HttpClient GetMockHttpClient(string urlContains, string responseContent)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains(urlContains)),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

        return new HttpClient(handlerMock.Object);
    }
}