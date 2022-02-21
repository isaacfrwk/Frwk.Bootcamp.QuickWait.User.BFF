using Confluent.Kafka;

namespace FrwkQuickWait.Domain.Intefaces
{
    public interface IConsumerService
    {
        Task<ConsumeResult<Ignore, string>> ProcessQueue(string topicName, CancellationToken cancellationToken);
    }
}
