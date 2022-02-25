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
                BootstrapServers = _configuration.GetSection("Kafka")["host"],
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
            consumer.Subscribe(topicName);

            try
            {

                while ((bool)!cancelled)
                {
                    try
                    {

                        consumeResult = consumer.Consume(cancellationToken);
                        cancelled = true;
                    }
                    catch (ConsumeException ex) { consumer.Close(); }

                }

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
