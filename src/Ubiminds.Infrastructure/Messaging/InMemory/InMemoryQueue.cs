using System.Collections.Concurrent;

namespace Ubiminds.Infrastructure.Messaging.InMemory;

public class InMemoryQueue
{
    private readonly SemaphoreSlim _signal = new(0);
    private readonly ConcurrentQueue<object> _queue = new();

    public void Enqueue(object message)
    {
        ArgumentNullException.ThrowIfNull(message);

        _queue.Enqueue(message);
        _signal.Release();
    }

    public async Task<object> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);

        _queue.TryDequeue(out var message);
        return message!;
    }
}