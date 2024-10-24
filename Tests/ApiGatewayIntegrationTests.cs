using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

public class ApiGatewayIntegrationTests
{
    private readonly HttpClient _client;

    public ApiGatewayIntegrationTests()
    {
        // This will vary depending on your test setup (use in-memory testing server, or live server)
        _client = new HttpClient { BaseAddress = new System.Uri("http://localhost:5000") };
    }

    [Fact]
    public async Task GetCatalogBrands_Should_Return_Ok()
    {
        // Act:
        // Make a GET request to the API Gateway's exposed endpoint
        var response = await _client.GetAsync("/gateway/catalog/catalog-brands");

        // Assert:
        // Check the response from the downstream service
        response.EnsureSuccessStatusCode(); // Asserts a 200-299 response
        var responseString = await response.Content.ReadAsStringAsync();
        Assert.NotNull(responseString);
    }

    [Fact]
    public async Task GetNonExistingRoute_Should_Return_404()
    {
        // Act:
        // Call a non-existing route
        var response = await _client.GetAsync("/gateway/catalog/non-existing-route");

        // Assert:
        // Expect a 404 status code
        Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
    }
}
