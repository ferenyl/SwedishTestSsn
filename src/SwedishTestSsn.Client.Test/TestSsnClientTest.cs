using Moq;
using Moq.Protected;
using System.Collections.Immutable;
using System.Net;

namespace SwedishTestSsn.Client.Test;

public class TestSsnClientTests
{
    [Fact]
    public async Task ReturnSingle()
    {
        // Din JSON som du vill returnera
        const string json = "{\"ResultCount\":1,\"Offset\":0,\"Limit\":1,\"QueryTime\":100,\"Next\":\"\",\"Results\":[{\"Testpersonnummer\":\"199001011234\"}]}";

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

        var sut = new TestSsnClient(httpClient);

        Result<ImmutableArray<string>> result = await sut.GetTestSsns("19900101", 1, 0, TestContext.Current.CancellationToken);

        Assert.Single(result.Value);
    }

    [Fact]
    public async Task ReturnEmpty()
    {
        // Din JSON som du vill returnera
        const string json = "{\"ResultCount\":1,\"Offset\":0,\"Limit\":1,\"QueryTime\":100,\"Next\":\"\",\"Results\":[]}";

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

        var sut = new TestSsnClient(httpClient);

        Result<ImmutableArray<string>> result = await sut.GetTestSsns("19900101", 1, 0, TestContext.Current.CancellationToken);

        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task ReturnsFailure_OnNonSuccessStatusCode()
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

        var sut = new TestSsnClient(httpClient);
        var result = await sut.GetTestSsns("19900101", 1, 0, TestContext.Current.CancellationToken);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ReturnsFailure_OnTimeout()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException("Timeout"));

        var httpClient = new HttpClient(handlerMock.Object);

        var sut = new TestSsnClient(httpClient);

        var result = await sut.GetTestSsns("19900101", 1, 0, TestContext.Current.CancellationToken);

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public async Task ReturnsFailure_OnInvalidJson()
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

        var sut = new TestSsnClient(httpClient);

        var result = await sut.GetTestSsns("19900101", 1, 0, TestContext.Current.CancellationToken);

        Assert.False(result.IsSuccess);
    }
}
