using Xunit;
using System.Collections.Generic;
using Moq;
using Moq.Protected;
using FluentAssertions;
using ApiGateway.Tests.Helpers;
using Ocelot.Configuration.File;
using ApiGateway.Tests.RouteValidationService;

namespace ApiGateway.Tests
{
    public class ApiGatewayConfigurationTests
    {

        public async Task<FileConfiguration> LoadConfig(string configRoutePath)
        {
            var config = await OceloteConfigHelper.LoadOcelotConfigAsync(configRoutePath);
            return config;
        }

        public FileRoute FindRoute(FileConfiguration config, string upstreamTemplate)
        { 
            FileRoute route = config.Routes.Find(r => r.UpstreamPathTemplate == upstreamTemplate);
            return route;
        }

        // I need a key value object bound to the UpStreamPathTemplate that returns a specific sets of validation based
        // on this template. 

        [Theory]
        [InlineData("/gateway/catalog/catalog-brands")]
        [InlineData("/gateway/catalog/catalog-items")]
        [InlineData("/gateway/catalog/catalog-items/{catalogItemId}")]
        [InlineData("/gateway/catalog/catalog-types")]
        public async Task CatalogRoutes_Should_Be_Correctly_Configured(string upstreamPath)
        {
            // Arrange:
            // Load the actual configuration file
            var config = await LoadConfig("../ApiGateway/RouteConfigs/catalog-routes.json");

            // Get the expected validation object for this route from the helper
            Dictionary<string, RouteValidation> validationRules = RouteValidationHelper.GetRouteValidationRules();
            validationRules.ContainsKey(upstreamPath).Should().BeTrue();

            var expectedValidation = validationRules[upstreamPath];

            // Act: Get the routes from the configuration
            var route = FindRoute(config,upstreamPath);

            // Assert:
            // Validate the route is found
            route.Should().NotBeNull();

            //Validate the downstream path
            route.DownstreamPathTemplate.Should().Be(expectedValidation.DownstreamPathTemplate);

            // Validate downstream scheme
            route.DownstreamScheme.Should().Be(expectedValidation.DownstreamScheme);

            // Validate the downstream host and port
            route.DownstreamHostAndPorts.Should().ContainSingle(hostAndPort =>
                hostAndPort.Host == expectedValidation.Host && hostAndPort.Port == expectedValidation.Port);

            // Validate the expected HTTP methods
            route.UpstreamHttpMethod.Should().BeEquivalentTo(expectedValidation.ExpectedMethods);
        }

        [Theory]
        [InlineData("/gateway/authentication/authenticate")]
        public async Task AuthenticationRoutes_Should_Be_Correctly_Configured(string upstreamPath)
        {
            // Arrange:
            // Load the actual configuration file
            var config = await LoadConfig("../ApiGateway/RouteConfigs/authentication-routes.json");

            // Get the expected validation object for this route from the helper
            Dictionary<string, RouteValidation> validationRules = RouteValidationHelper.GetRouteValidationRules();
            validationRules.ContainsKey(upstreamPath).Should().BeTrue();

            var expectedValidation = validationRules[upstreamPath];

            // Act: Get the routes from the configuration
            var route = FindRoute(config, upstreamPath);

            // Assert:
            // Validate the route is found
            route.Should().NotBeNull();

            //Validate the downstream path
            route.DownstreamPathTemplate.Should().Be(expectedValidation.DownstreamPathTemplate);

            // Validate downstream scheme
            route.DownstreamScheme.Should().Be(expectedValidation.DownstreamScheme);

            // Validate the downstream host and port
            route.DownstreamHostAndPorts.Should().ContainSingle(hostAndPort =>
                hostAndPort.Host == expectedValidation.Host && hostAndPort.Port == expectedValidation.Port);

            // Validate the expected HTTP methods
            route.UpstreamHttpMethod.Should().BeEquivalentTo(expectedValidation.ExpectedMethods);
        }
    }
}