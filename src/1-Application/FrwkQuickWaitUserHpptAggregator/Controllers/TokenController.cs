using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace FrwkQuickWaitUserHpptAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IProducerService producerService;
        private readonly IConsumerService consumerService;
        public TokenController(IProducerService producerService, IConsumerService consumerService)
        {
            this.producerService = producerService;
            this.consumerService = consumerService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            var message = new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(user));

            await producerService.Call(message, Topics.AUTH);

            var response = await consumerService.ProcessQueue(Topics.AUTHRESPONSE);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            if (input.Status == 404)
                return NotFound(new { token = input.Content });

            return Ok(new { token = input.Content });
        }
    }
}
