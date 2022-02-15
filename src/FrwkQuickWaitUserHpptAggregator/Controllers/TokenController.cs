using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] User user)
        {
            var message = new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(user));

            await producerService.Call(message, Topics.AUTH);

            var response = await consumerService.ProcessQueue(Topics.AUTHRESPONSE);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            var token = input.Content.Replace("\"", "");

            if (input.Status == 404)
                return NotFound(new { token });

            return Ok(new { token });
        }
    }
}
