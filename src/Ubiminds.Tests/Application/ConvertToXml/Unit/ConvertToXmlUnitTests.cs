using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Ubiminds.Domain.Models.InputModels;
using Ubiminds.Infrastructure.Configuration;
using Ubiminds.Infrastructure.Interfaces;
using Ubiminds.Infrastructure.Messaging.InMemory;
using Xunit;
using Assert = Xunit.Assert;

namespace Ubiminds.Tests.Application.ConvertToXml.Unit;

public class ConvertToXmlUnitTests : IDisposable
{
    private readonly string _tempDir;

    public ConvertToXmlUnitTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"ubiminds-tests-{DateTime.UtcNow:yyyyMMddHHmmss}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (!Directory.Exists(_tempDir)) return;
        foreach (var file in Directory.GetFiles(_tempDir))
            File.Delete(file);

        Directory.Delete(_tempDir, recursive: true);
    }

    [Fact(DisplayName = "Should generate XML file successfully")]
    public async Task ProcessMessageAsync_ShouldGenerateXml()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        xmlConverter.Setup(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()))
            .Returns("<xml>valid</xml>");

        var input = new DocumentInputModel
        {
            Status = 3,
            PublishDate = DateTime.UtcNow,
            TestRun = true,
            Title = "Valid"
        };

        var queue = new InMemoryQueue();
        queue.Enqueue(input);

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Setup(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()))
            .Returns("<xml>valid</xml>");
        var files = Directory.GetFiles(_tempDir);
        Assert.Single(files);

        // Cleanup extra-safe
        foreach (var file in files)
            File.Delete(file);
    }

    [Fact(DisplayName = "Should skip message when input is not InputData")]
    public async Task ProcessMessageAsync_ShouldSkipInvalidMessage()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();
        queue.Enqueue("not an InputData");

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }

    [Fact(DisplayName = "Should skip message when PublishDate is in the future")]
    public async Task ProcessMessageAsync_ShouldSkipWhenPublishDateInFuture()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();

        var input = new DocumentInputModel
        {
            Status = 3,
            PublishDate = new DateTime(2024, 08, 01),
            TestRun = true,
            Title = "Future",
        };

        queue.Enqueue(input);

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }

    [Fact(DisplayName = "Should skip message when TestRun is false")]
    public async Task ProcessMessageAsync_ShouldSkipWhenTestRunIsFalse()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();

        var input = new DocumentInputModel
        {
            Status = 3,
            PublishDate = DateTime.UtcNow,
            TestRun = false, // Not a test run
            Title = "NotTest"
        };

        queue.Enqueue(input);

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }

    [Fact(DisplayName = "Should not generate XML if status is not 3")]
    public async Task ProcessMessageAsync_ShouldNotGenerateXml_WhenStatusIsInvalid()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();
        queue.Enqueue(new DocumentInputModel
        {
            Status = 2, // invalid
            PublishDate = new DateTime(2024, 08, 24),
            TestRun = true
        });

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }

    [Fact(DisplayName = "Should not generate XML if publish date is before 2024-08-24")]
    public async Task ProcessMessageAsync_ShouldNotGenerateXml_WhenPublishDateIsTooOld()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();
        queue.Enqueue(new DocumentInputModel
        {
            Status = 3,
            PublishDate = new DateTime(2024, 08, 23), // too old
            TestRun = true
        });

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }

    [Fact(DisplayName = "Should not generate XML if TestRun is false")]
    public async Task ProcessMessageAsync_ShouldNotGenerateXml_WhenTestRunIsFalse()
    {
        // Arrange
        var logger = Mock.Of<ILogger<InMemoryBackgroundConsumer>>();
        var xmlConverter = new Mock<IXmlConverterService>();
        var queue = new InMemoryQueue();
        queue.Enqueue(new DocumentInputModel
        {
            Status = 3,
            PublishDate = new DateTime(2024, 08, 25),
            TestRun = false // not test
        });

        var options = Options.Create(new XmlOutputSettings
        {
            OutputDirectory = _tempDir
        });

        var consumer = new InMemoryBackgroundConsumer(queue, logger, xmlConverter.Object, options);

        // Act
        using var cts = new CancellationTokenSource(millisecondsDelay: 500);
        await consumer.StartAsync(cts.Token);

        // Assert
        xmlConverter.Verify(x => x.ConvertToXml(It.IsAny<DocumentInputModel>()), Times.Never);
        Assert.Empty(Directory.GetFiles(_tempDir));
    }
}