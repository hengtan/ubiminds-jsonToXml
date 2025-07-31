using System.Text;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ubiminds.Domain.Models.InputModels;
using Ubiminds.Infrastructure.Configuration;
using Ubiminds.Infrastructure.Xml.Interfaces;

namespace Ubiminds.Infrastructure.Messaging.InMemory;

public sealed class InMemoryBackgroundConsumer(
    InMemoryQueue queue,
    ILogger<InMemoryBackgroundConsumer> logger,
    IXmlConverter xmlConverter,
    IOptions<XmlOutputSettings> outputSettings)
    : BackgroundService
{
    private readonly InMemoryQueue _queue = queue ?? throw new ArgumentNullException(nameof(queue));
    private readonly ILogger<InMemoryBackgroundConsumer> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IXmlConverter _xmlConverter = xmlConverter ?? throw new ArgumentNullException(nameof(xmlConverter));
    private readonly string _outputDirectory = ResolveOutputPath(outputSettings?.Value?.OutputDirectory);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("In-memory XML consumer started. Output directory: {OutputDirectory}", _outputDirectory);

        EnsureOutputDirectoryExists();

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = await _queue.DequeueAsync(stoppingToken);

            if (message is not DocumentInputModel inputData)
            {
                _logger.LogWarning("Skipped message: expected {ExpectedType} but received {ActualType}",
                    nameof(DocumentInputModel), message?.GetType().Name ?? "null");
                continue;
            }

            _logger.LogInformation("Processing message: Title={Title}, Status={Status}, PublishDate={PublishDate}",
                inputData.Title, inputData.Status, inputData.PublishDate);

            await ProcessMessageAsync(inputData, stoppingToken);
        }

        _logger.LogInformation("In-memory consumer has stopped.");
    }

    private async Task ProcessMessageAsync(DocumentInputModel data, CancellationToken token)
    {
        if (!data.IsValidForXml())
        {
            _logger.LogWarning("Skipped XML generation: business rule not satisfied | Status={Status}, PublishDate={PublishDate}",
                data.Status, data.PublishDate);
            return;
        }

        try
        {
            var xml = _xmlConverter.Serialize(data);
            var filePath = GenerateFilePathWithTimestamp(data.Title);

            await File.WriteAllTextAsync(filePath, xml, Encoding.UTF8, token);

            _logger.LogInformation("✅ XML file successfully generated at: {FilePath}", filePath);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while generating XML for Title={Title}", data.Title);
        }
    }

    private static string ResolveOutputPath(string? relativePath)
    {
        var projectRoot = Directory.GetParent(AppContext.BaseDirectory)!.Parent!.Parent!.FullName;
        return Path.Combine(projectRoot, relativePath ?? "output");
    }

    private void EnsureOutputDirectoryExists()
    {
        if (!Directory.Exists(_outputDirectory))
        {
            Directory.CreateDirectory(_outputDirectory);
            _logger.LogInformation("📁 Output directory created at: {OutputDirectory}", _outputDirectory);
        }
    }

    private string GenerateFilePathWithTimestamp(string title)
    {
        var safeTitle = string.Concat(title.Where(char.IsLetterOrDigit));
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var fileName = $"ubiminds-{safeTitle}-{timestamp}.xml";
        return Path.Combine(_outputDirectory, fileName);
    }
}