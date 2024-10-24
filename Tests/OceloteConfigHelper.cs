using Ocelot.Configuration.File;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Threading.Tasks;
using System.IO;

namespace ApiGateway.Tests.Helpers
{
    public class OceloteConfigHelper
    {
        public static async Task<FileConfiguration> LoadOcelotConfigAsync(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var config = await JsonSerializer.DeserializeAsync<FileConfiguration>(stream);
                return config;
            }
        }
    }
}
