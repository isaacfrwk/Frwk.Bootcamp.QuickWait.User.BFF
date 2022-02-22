using FrwkQuickWait.Domain.Constants;
using FrwkQuickWait.Domain.Entity;
using FrwkQuickWait.Domain.Intefaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FrwkQuickWaitUserHpptAggregator.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IProducerService producerService;
        private readonly IConsumerService consumerService;
        //private readonly IConfiguration configuration;

        public TokenController(IProducerService producerService,
                               IConsumerService consumerService)
        {
            this.producerService = producerService;
            this.consumerService = consumerService;
            //this.configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] UserAuth model)
        {

            //using (SentrySdk.Init(o =>
            //{
            //    o.Dsn = configuration.GetSection("Sentry")["Dsn"];
            //    o.Debug = true;
            //    o.TracesSampleRate = 1.0;
            //}))
            //{
                MessageInput? input = null;
                string? access_token = null;

                try
                {
                    var message = new MessageInput(null, Methods.POST, JsonConvert.SerializeObject(model));

                    await producerService.Call(message, Topics.AUTH);

                    var response = await consumerService.ProcessQueue(Topics.AUTHRESPONSE);

                    input = JsonConvert.DeserializeObject<MessageInput>(response.Message.Value);

                    access_token = input.Content.Replace("\"", "");

                }
                catch (Exception ex)
                {
                    SentrySdk.CaptureException(ex);
                }

                if (input.Status == 404)
                    return NotFound(new { access_token });

                return Ok(new { access_token });

            //}

        }
    }
}
