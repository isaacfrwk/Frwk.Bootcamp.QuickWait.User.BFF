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
    public class UserController : ControllerBase
    {
        private readonly IProducerService producerService;
        private readonly IConsumerService consumerService;
        public UserController(IProducerService producerService, IConsumerService consumerService)
        {
            this.producerService = producerService;
            this.consumerService = consumerService;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var message = new MessageInput(null, Methods.GETBYID, string.Empty);

            await producerService.Call(message, Topics.USER);

            var response = await consumerService.ProcessQueue(Topics.USERRESPONSE);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            if (input.Status == 404)
                return NotFound(new { token = input.Content });

            return Ok(new { user = input.Content });
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute][Required] Guid id)
        {
            var message = new MessageInput(null, Methods.GETBYID, JsonConvert.SerializeObject(id));

            await producerService.Call(message, Topics.USER);

            var response = await consumerService.ProcessQueue(Topics.USERRESPONSE);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            if (input.Status == 404)
                return NotFound(new { token = input.Content });

            return Ok(new { user = input.Content });

        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody][Required] User user)
        {
            var message = new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(user));

            await producerService.Call(message, Topics.USER);

            return Created($"{ Request.Path }", null);
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromBody][Required] User user)
        {
            var message = new MessageInput(null, Methods.PUT, JsonConvert.SerializeObject(user));

            await producerService.Call(message, Topics.USER);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void Delete([FromBody][Required] User user)
        {
            var message = new MessageInput(null, Methods.DELETE, JsonConvert.SerializeObject(user));

            producerService.Call(message, Topics.USER);
        }
    }
}
