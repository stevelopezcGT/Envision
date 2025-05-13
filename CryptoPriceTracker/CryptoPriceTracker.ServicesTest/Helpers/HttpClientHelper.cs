namespace CryptoPriceTracker.ApplicationTest.Helpers;

public static class HttpClientHelper
{
    public static HttpClient GetMockHttpClient(HttpResponseMessage response, Func<HttpRequestMessage, bool> match = null)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);
        return new HttpClient(handlerMock.Object);
    }
}