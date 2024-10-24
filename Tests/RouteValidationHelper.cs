using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiGateway.Tests.RouteValidationService
{
    public class RouteValidation
    {
        public string DownstreamPathTemplate { get; set; }
        public string DownstreamScheme { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public List<string> ExpectedMethods { get; set; }
    }

    public static class RouteValidationHelper
    {
        // Dictionary that maps the UpstreamPathTemplate to its expected validation rules
        public static Dictionary<string, RouteValidation> GetRouteValidationRules()
        {
            return new Dictionary<string, RouteValidation>
            {
                {
                    "/gateway/catalog/catalog-brands",
                    new RouteValidation
                    {
                        DownstreamPathTemplate = "/api/catalog-brands",
                        DownstreamScheme = "http",
                        Host = "localhost",
                        Port = 5001,
                        ExpectedMethods = new List<string> { "Get" }
                    }
                },
                {
                    "/gateway/catalog/catalog-items",
                    new RouteValidation
                    {
                        DownstreamPathTemplate = "/api/catalog-items",
                        DownstreamScheme = "http",
                        Host = "localhost",
                        Port = 5001,
                        ExpectedMethods = new List<string> { "Get", "Post", "Put" }
                    }
                },
                {
                    "/gateway/catalog/catalog-items/{catalogItemId}",
                    new RouteValidation
                    {
                        DownstreamPathTemplate = "/api/catalog-items/{catalogItemId}",
                        DownstreamScheme = "http",
                        Host = "localhost",
                        Port = 5001,
                        ExpectedMethods = new List<string> { "Get", "Delete" }
                    }
                },
                {
                    "/gateway/catalog/catalog-types",
                    new RouteValidation
                    {
                        DownstreamPathTemplate = "/api/catalog-types",
                        DownstreamScheme = "http",
                        Host = "localhost",
                        Port = 5001,
                        ExpectedMethods = new List<string> { "Get" }
                    }
                }
            };
        }
    }
}
