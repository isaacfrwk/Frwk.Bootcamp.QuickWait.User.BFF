using FrwkQuickWait.Domain.Entity;

namespace FrwkQuickWait.Domain.Intefaces
{
    public interface IProducerService
    {
        Task Call(MessageInput message, string topicName);
    }
}
