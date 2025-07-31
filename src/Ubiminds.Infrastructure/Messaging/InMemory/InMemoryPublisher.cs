using Ubiminds.Domain.Interfaces;

namespace Ubiminds.Infrastructure.Messaging.InMemory;

public class InMemoryPublisher(InMemoryQueue queue) : IMessagePublisher
{
    public Task PublishAsync(string topic, object message)
    {
        queue.Enqueue(message);
        return Task.CompletedTask;
    }
}