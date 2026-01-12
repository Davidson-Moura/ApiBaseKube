using Common;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Tests;

public class WebTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Test]
    public async Task GetWebResourceRootReturnsOkStatusCode()
    {
        var cancellationToken = TestContext.CurrentContext.CancellationToken;

        var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.ApiService>(cancellationToken);
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
        });
        appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
        {
            clientBuilder.AddStandardResilienceHandler();
        });

        await using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        await app.StartAsync(cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);

        var httpClient = app.CreateHttpClient("webfrontend");
        await app.ResourceNotifications.WaitForResourceHealthyAsync("webfrontend", cancellationToken).WaitAsync(DefaultTimeout, cancellationToken);
        var response = await httpClient.GetAsync("/", cancellationToken);

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    [Test]
    [Ignore("Apenas para gerar chaves")]
    public async Task GenerateKey()
    {
        //var bytes = RandomNumberGenerator.GetBytes(32);
        //var secretKey = Convert.ToBase64String(bytes);

        var keyBytes = RandomNumberGenerator.GetBytes(64);
        var secretKey = Convert.ToBase64String(keyBytes);
    }

    [Test]
    [Ignore("Apenas para criptografar")]
    public async Task GeneratePassword()
    {
        var hash = Cryptography.HashPassword("");
    }
}
