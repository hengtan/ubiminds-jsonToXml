using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Ubiminds.Domain.Models.InputModels;
using Assert = Xunit.Assert;

namespace Ubiminds.Tests.Application.ConvertToXml.Integration;

public class ConvertToXmlIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly string _outputDir;

    public ConvertToXmlIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();

        _outputDir = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "../../../../Ubiminds.Tests/bin/output")
        );
        Directory.CreateDirectory(_outputDir);

        Environment.SetEnvironmentVariable("XmlOutputSettings__OutputDirectory", _outputDir);
    }

    public void Dispose()
    {
        if (!Directory.Exists(_outputDir)) return;

        foreach (var file in Directory.GetFiles(_outputDir))
            File.Delete(file);

        Directory.Delete(_outputDir, recursive: true);
    }

    #region Helpers

    private static DocumentInputModel CreateInputData(int status, DateTime publishDate, bool testRun,
        string? title = null, string? description = null) =>
        new()
        {
            Status = status,
            PublishDate = publishDate,
            TestRun = testRun,
            Title = title ?? "Default",
        };

    private async Task<HttpResponseMessage> PostAsync(object input) =>
        await _client.PostAsJsonAsync("/api/Xml", input);

    private string[] GetGeneratedFiles(int maxWaitMs = 2000)
    {
        var deadline = DateTime.UtcNow.AddMilliseconds(maxWaitMs);
        while (DateTime.UtcNow < deadline)
        {
            var files = Directory.GetFiles(_outputDir);
            if (files.Length > 0) return files;
            Thread.Sleep(100);
        }

        return [];
    }

    #endregion

    #region Tests

    [Fact(DisplayName = "Returns 200 and generates XML when input is valid")]
    public async Task ShouldReturnSuccess_WhenValidInputIsPosted()
    {
        // Arrange
        var input = new DocumentInputModel
        {
            Status = 3,
            PublishDate = DateTime.Parse("2024-08-25T10:00:00Z"),
            TestRun = true,
            Title = "Ubiminds XML Test"
        };

        // Act
        var response = await PostAsync(input);

        // Assert
        response.EnsureSuccessStatusCode();

        var files = GetGeneratedFiles(maxWaitMs: 3000);
        Assert.Single(files);
    }

    [Fact(DisplayName = "Returns 400 when required fields are missing")]
    public async Task ShouldReturnBadRequest_WhenInvalidModelIsPosted()
    {
        var input = new { Status = 3 };

        var response = await PostAsync(input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact(DisplayName = "Skips XML when TestRun is false")]
    public async Task ShouldNotGenerateXml_WhenTestRunIsFalse()
    {
        var input = CreateInputData(3, new DateTime(2024, 08, 25), testRun: false);

        var response = await PostAsync(input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(GetGeneratedFiles());
    }

    [Fact(DisplayName = "Skips XML when Status != 3")]
    public async Task ShouldNotGenerateXml_WhenStatusIsNotThree()
    {
        var input = CreateInputData(status: 2, new DateTime(2024, 08, 25), testRun: true);

        var response = await PostAsync(input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(GetGeneratedFiles());
    }

    [Fact(DisplayName = "Skips XML when PublishDate < 2024-08-24")]
    public async Task ShouldNotGenerateXml_WhenPublishDateIsTooEarly()
    {
        var input = CreateInputData(
            status: 3,
            publishDate: new DateTime(2024, 08, 20),
            testRun: true,
            title: "TooEarly",
            description: "Should be skipped due to early publish date"
        );

        var response = await PostAsync(input);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Empty(GetGeneratedFiles());
    }

    #endregion
}