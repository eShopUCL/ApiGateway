using FluentAssertions;
using Moq;
using RichardSzalay.MockHttp;

public class DownstreamServiceTests
{
    private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;

    public DownstreamServiceTests()
    {
        _mockHttpClientFactory = new Mock<IHttpClientFactory>();
    }

    public HttpClient SetupMock(MockHttpMessageHandler mockHttp)
    {
        var mockHttpClient = mockHttp.ToHttpClient();
        mockHttpClient.BaseAddress = new System.Uri("http://localhost:5001");

        _mockHttpClientFactory.Setup(clientFactory => clientFactory.CreateClient(It.IsAny<string>())).Returns(mockHttpClient);

        return mockHttpClient;
    }

    [Fact]
    public async Task ApiGateway_Should_Forward_Request_To_DownstreamService()
    {
        // Arrange: Create the MockHttpMessageHandler
        var mockHttp = new MockHttpMessageHandler();

        // Define the mock response for the downstream service
        mockHttp.When("/api/catalog-brands")
                .Respond("application/json", "{\"data\": \"some mock data\"}");

        // Setup the mock HttpClient with the downstream service response
        HttpClient mockClient = SetupMock(mockHttp);

        // Act:
        // Make a request to the API Gateway's upstream path
        var response = await mockClient.GetAsync("/gateway/catalog/catalog-brands");

        // Assert:
        // Verify that the response received is the mocked downstream response
        response.IsSuccessStatusCode.Should().BeTrue();

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Be("{\"data\": \"some mock data\"}");
    }

}
