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
        private readonly IConfiguration configuration;
        public UserController(IProducerService producerService, IConsumerService consumerService, IConfiguration configuration)
        {
            this.producerService = producerService;
            this.consumerService = consumerService;
            this.configuration = configuration;
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(UserProfile), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var message = new MessageInput(null, Methods.FINDALL, string.Empty);

            await producerService.Call(message, Topics.USER);

            var response = await consumerService.ProcessQueue(Topics.USERRESPONSE, cancellationToken);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

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
        public async Task<IActionResult> Get([FromRoute][Required] Guid id, CancellationToken cancellationToken)
        {
            var message = new MessageInput(null, Methods.GETBYID, JsonConvert.SerializeObject(id));

            await producerService.Call(message, Topics.USER);

            var response = await consumerService.ProcessQueue(Topics.USERRESPONSE, cancellationToken);

            var input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

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
        public async Task<IActionResult> Post([FromBody][Required] UserProfile model, CancellationToken cancellationToken)
        {
            using (SentrySdk.Init(o =>
            {
                o.Dsn = configuration.GetSection("Sentry")["Dsn"];
                o.Debug = true;
                o.TracesSampleRate = 1.0;
            }))
            {
                MessageInput? input = null;
                UserProfile? user = null;

                try
                {

                    var message = new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(model));

                    await producerService.Call(message, Topics.USER);

                    var response = await consumerService.ProcessQueue(Topics.USERRESPONSE, cancellationToken);

                    input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

                    user = JsonConvert.DeserializeObject<UserProfile>(input.Content.Replace("\"",""));

                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }
                
                if (input.Status == 400)
                    return BadRequest(new { user });

                return Created($"{ Request.Path }", new { user });

            }
        }

        [HttpPut]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(User), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Put([FromBody][Required] UserProfile user)
        {
            var message = new MessageInput(null, Methods.PUT, JsonConvert.SerializeObject(user));

            await producerService.Call(message, Topics.USER);

            return NoContent();
        }

        [HttpDelete]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public void Delete([FromBody][Required] UserProfile user)
        {
            var message = new MessageInput(null, Methods.DELETE, JsonConvert.SerializeObject(user));

            producerService.Call(message, Topics.USER);
        }
    }
}
