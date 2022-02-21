using Confluent.Kafka;
using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.Extensions.Configuration;

namespace FrwkQuickWait.Service.Services
{
    public class ConsumerService : IConsumerService
    {
        private readonly ConsumerConfig config;
        private readonly IConfiguration _configuration;
        public ConsumerService(IConfiguration configuration)
        {
            _configuration = configuration;

            this.config = new ConsumerConfig
            {
                BootstrapServers = _configuration.GetSection("Kafka")["Host"],
                //SaslUsername = CloudKarafka.Username,
                //SaslPassword = CloudKarafka.Password,
                //SaslMechanism = SaslMechanism.ScramSha256,
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                EnableAutoOffsetStore = false,
                AutoOffsetReset = AutoOffsetReset.Earliest
            }; 
        }

        public async Task<ConsumeResult<Ignore, string>> ProcessQueue(string topicName, CancellationToken cancellationToken)
        {

            var consumeResult = new ConsumeResult<Ignore, string>();
            config.GroupId = $"{topicName}-group-2";
            using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
            //var topic = String.Concat(CloudKarafka.Prefix, topicName);
            consumer.Subscribe(topicName);
            
            try
            {
                //while (!cancellationToken.IsCancellationRequested)
                //{

                    consumeResult = consumer.Consume(cancellationToken);
                    consumer.StoreOffset(consumeResult);

                //}

                consumer.Close();

            }
            catch (OperationCanceledException ex)
            {
                consumer.Close();
            }

            return await Task.FromResult(consumeResult);
        }
    }
}
