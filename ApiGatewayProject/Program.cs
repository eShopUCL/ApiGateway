using System.IO;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Linq;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Følgende kode er tilføjet så vi dynamisk kan ændre de routes som vores
// api gateway bruger, alt efter om vi er i gang med udvikling, eller
// om vi er i et produktionsmiljø (kører deploy-api-gateway.sh)

// Find ud af hvilket environment vi er i, Production eller Development
var environment = builder.Environment.EnvironmentName;

// Vælg den ocelot fil, som passer til vores environment
string ocelotConfigPath = $"ocelot.{environment}.json";

// Åbn config filen og læs indholdet af den
var ocelotConfig = JObject.Parse(File.ReadAllText(ocelotConfigPath));
builder.Configuration.AddJsonFile(ocelotConfigPath, optional: false, reloadOnChange: true);

// Tilføj Ocelot Service
builder.Services.AddOcelot();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Vi bliver nødt til at sætte PathBase når vi kører i et produktions env
// da vores ingress forventer, at vi har prefixes på vores services.
if (environment.Equals("Production", StringComparison.OrdinalIgnoreCase))
{
    app.UsePathBase("/apigateway");
}

app.UseSwagger();

// Sætter swagger UI op, så vi kan se om servicen faktisk er
// oppe og køre, denne sættes også dynamisk alt efter om der skal prefix på eller ej.
app.UseSwaggerUI(c =>
{
    var swaggerEndpoint = environment.Equals("Production", StringComparison.OrdinalIgnoreCase)
        ? "/apigateway/swagger/v1/swagger.json"
        : "/swagger/v1/swagger.json";

    c.SwaggerEndpoint(swaggerEndpoint, "Catalog Service API V1");
    c.RoutePrefix = "swagger"; // Accessible at /swagger or /apigateway/swagger
});

// app.UseHttpsRedirection(); // Udkommenteret httpsredirection da vi ikke bruger HTTPS
app.UseAuthorization();

// Ocelot middleware
await app.UseOcelot();

app.MapControllers();

app.Run();
