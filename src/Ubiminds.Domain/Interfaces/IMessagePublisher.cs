namespace Ubiminds.Domain.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync(string topic, object message);
}