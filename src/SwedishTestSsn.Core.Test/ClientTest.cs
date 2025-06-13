using Moq;
using Moq.Protected;
using System.Collections.Immutable;
using System.Net;

namespace SwedishTestSsn.Core.Test;

public class ClientTests
{
    [Fact]
    public async Task ReturnSingle()
    {
        // Din JSON som du vill returnera
        var json = "{\"ResultCount\":1,\"Offset\":0,\"Limit\":1,\"QueryTime\":100,\"Next\":\"\",\"Results\":[{\"Testpersonnummer\":\"199001011234\"}]}";

        // Mocka HttpMessageHandler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json),
            });

        // Skapa HttpClient med mockad handler
        var httpClient = new HttpClient(handlerMock.Object);

        // Mocka IHttpClientFactory
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var sut = new Client(httpClientFactoryMock.Object);

        ImmutableArray<string> result = await sut.GetTestSsns("19900101", 1, 0);

        Assert.Single(result);
    }

    [Fact]
    public async Task ReturnEmpty()
    {
        // Din JSON som du vill returnera
        var json = "{\"ResultCount\":1,\"Offset\":0,\"Limit\":1,\"QueryTime\":100,\"Next\":\"\",\"Results\":[]}";

        // Mocka HttpMessageHandler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json),
            });

        // Skapa HttpClient med mockad handler
        var httpClient = new HttpClient(handlerMock.Object);

        // Mocka IHttpClientFactory
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var sut = new Client(httpClientFactoryMock.Object);

        ImmutableArray<string> result = await sut.GetTestSsns("19900101", 1, 0);

        Assert.Empty(result);
    }

    [Fact]
    public async Task Throws_OnNonSuccessStatusCode()
    {
        // Mocka HttpMessageHandler för att returnera 404
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("Error"),
            });

        var httpClient = new HttpClient(handlerMock.Object);

        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock
            .Setup(f => f.CreateClient(It.IsAny<string>()))
            .Returns(httpClient);

        var sut = new Client(httpClientFactoryMock.Object);
        await Assert.ThrowsAsync<HttpRequestException>(async () => await sut.GetTestSsns("19900101", 1, 0));
    }

    [Fact]
    public async Task ThrowsTaskCanceledException_OnTimeout()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var sut = new Client(httpClientFactoryMock.Object);

        await Assert.ThrowsAsync<TaskCanceledException>(() => sut.GetTestSsns("19900101", 1, 0));
    }

    [Fact]
    public async Task ThrowsJsonException_OnInvalidJson()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("NOT JSON!"),
            });

        var httpClient = new HttpClient(handlerMock.Object);
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var sut = new Client(httpClientFactoryMock.Object);

        await Assert.ThrowsAsync<System.Text.Json.JsonException>(() => sut.GetTestSsns("19900101", 1, 0));
    }
}
