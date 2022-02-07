using Confluent.Kafka;
using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrwkQuickWait.Service.Services
{
    public class ProducerService : IProducerService
    {
        private readonly ClientConfig cloudConfig;
        public ProducerService()
        {
            cloudConfig = new ClientConfig
            {
                BootstrapServers = CloudKarafka.Brokers,
                SaslUsername = CloudKarafka.Username,
                SaslPassword = CloudKarafka.Password,
                SaslMechanism = SaslMechanism.ScramSha256,
                SecurityProtocol = SecurityProtocol.SaslSsl,
                EnableSslCertificateVerification = false
            };
        }

        public async Task Call(MessageInput message, string topicName)
        {
            var stringfiedMessage = JsonConvert.SerializeObject(message);

            using var producer = new ProducerBuilder<string, string>(cloudConfig).Build();

            var key = new Guid().ToString();

            await producer.ProduceAsync($"{CloudKarafka.Prefix + topicName}", new Message<string, string> { Key = key, Value = stringfiedMessage });

            producer.Flush(TimeSpan.FromSeconds(2));
        }
    }
}
