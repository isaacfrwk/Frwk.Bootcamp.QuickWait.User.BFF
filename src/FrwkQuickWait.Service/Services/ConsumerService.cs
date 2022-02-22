using Confluent.Kafka;
using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.Extensions.Configuration;

namespace FrwkQuickWait.Service.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ConsumerConfig config;
        
        public ConsumerService()
        {
            this.config = new ConsumerConfig
            {
                BootstrapServers = Settings.kafkahost,
                //SaslUsername = CloudKarafka.Username,
                //SaslPassword = CloudKarafka.Password,
                //SaslMechanism = SaslMechanism.ScramSha256,
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                EnableAutoOffsetStore = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            }; 
        }

        public async Task<ConsumeResult<Ignore, string>> ProcessQueue(string topicName)
        {
            bool? cancelled = false;
            var cancellationToken = new CancellationToken();
            var consumeResult = new ConsumeResult<Ignore, string>();
            config.GroupId = $"{topicName}-group-2";
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            //var topic = String.Concat(CloudKarafka.Prefix, topicName);
            consumer.Subscribe(topicName);
            
            try
            {
                try
                {
                    while ((bool)!cancelled)
                    {

                        consumeResult = consumer.Consume(cancellationToken);
                        consumer.StoreOffset(consumeResult);
                        cancelled = true;

                    }

                    consumer.Close();

                }
                catch (KafkaException ex) { }
                
            }
            catch (OperationCanceledException ex)
            {
                consumer.Close();
            }

            return await Task.FromResult(consumeResult);
        }
    }
}
