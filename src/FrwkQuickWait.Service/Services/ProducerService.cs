using Confluent.Kafka;
using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrwkQuickWait.Service.Services
{
    public class ProducerService : IProducerService
    {
        private readonly ClientConfig cloudConfig;
        private readonly IConfiguration _configuration;
        public ProducerService(IConfiguration configuration)
        {
            _configuration = configuration;

            cloudConfig = new ClientConfig
            {
                BootstrapServers = _configuration.GetSection("Kafka")["Host"]
                //SaslUsername = CloudKarafka.Username,
                //SaslPassword = CloudKarafka.Password,
                //SaslMechanism = SaslMechanism.ScramSha256,
                //SecurityProtocol = SecurityProtocol.SaslSsl,
                //EnableSslCertificateVerification = false
            };
        }

        public async Task Call(MessageInput message, string topicName)
        {
            var stringfiedMessage = JsonConvert.SerializeObject(message);

            using var producer = new ProducerBuilder<string, string>(cloudConfig).Build();

            var key = new Guid().ToString();

            await producer.ProduceAsync(topicName, new Message<string, string> { Key = key, Value = stringfiedMessage });

            producer.Flush(TimeSpan.FromSeconds(2));

        }
    }
}
