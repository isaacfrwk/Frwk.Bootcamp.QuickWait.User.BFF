using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            MessageInput? input = null;

            try
            {
                await producerService.Call(new MessageInput(null, Methods.FINDALL, string.Empty), Topics.USER);

                var response = await consumerService.ProcessQueue(Topics.USERRESPONSE);

                input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
            
            if (input.Status == 404)
                return NotFound(new { user = input.Content });

            var users = JsonConvert.DeserializeObject<IEnumerable<UserProfile>>(input.Content);

            return Ok(new { users });
        }

        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute][Required] Guid id)
        {
            MessageInput? input;

            try
            {

                await producerService.Call(new MessageInput(null, Methods.GETBYID, JsonConvert.SerializeObject(id)), Topics.USER);

                var response = await consumerService.ProcessQueue(Topics.USERRESPONSE);

                input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }
            

            if (input.Status == 404)
                return NotFound(new { user = input.Content });

            var user = JsonConvert.DeserializeObject<UserProfile>(input.Content);

            return Ok(new { user });

        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status201Created)]
        public async Task<IActionResult> Post([FromBody][Required] UserProfile model)
        {
            UserProfile? user;
            MessageInput? input;

            try
            {
                await producerService.Call(new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(model)), Topics.USER);

                var response = await consumerService.ProcessQueue(Topics.USERRESPONSE);

                input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }

            if (input.Status == 400)
                return BadRequest(new { user = input.Content.Replace("\"", "") });

            user = JsonConvert.DeserializeObject<UserProfile>(input.Content);

            return Created($"{ Request.Path }", new { user });
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromBody][Required] UserProfile user)
        {
            try
            {
                await producerService.Call(new MessageInput(null, Methods.PUT, JsonConvert.SerializeObject(user)), Topics.USER);
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                return BadRequest(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void Delete([FromBody][Required] UserProfile user)
        {
            try
            {
                producerService.Call(new MessageInput(null, Methods.DELETE, JsonConvert.SerializeObject(user)), Topics.USER);

            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }
        }
    }
}
