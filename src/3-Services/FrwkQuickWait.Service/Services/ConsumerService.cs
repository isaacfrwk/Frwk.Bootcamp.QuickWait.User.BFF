using Confluent.Kafka;
using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Intefaces;

namespace FrwkQuickWait.Service.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ConsumerConfig config;
        public ConsumerService()
        {
            this.config = new ConsumerConfig
            {
                BootstrapServers = CloudKarafka.Brokers,
                SaslUsername = CloudKarafka.Username,
                SaslPassword = CloudKarafka.Password,
                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                AutoOffsetReset = AutoOffsetReset.Earliest
            }; 
        }

        public async Task<ConsumeResult<Ignore, string>> ProcessQueue(string topicName)
        {

            var consumeResult = new ConsumeResult<Ignore, string>();
            config.GroupId = $"{topicName}-group-2";
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            consumer.Subscribe($"{CloudKarafka.Prefix + topicName}");
            var cancellationTokenSource = new CancellationTokenSource();
            
            try
            {
                consumeResult = consumer.Consume(cancellationTokenSource.Token);
                consumer.Close();
            }
            catch (OperationCanceledException ex)
            {
                
            }

            return await Task.FromResult(consumeResult);
        }
    }
}
