using DBQueue.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Text.Json.Serialization;

namespace GINotificationManager.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificheGIDispatcherController : ControllerBase
    {
        private readonly ILogger<NotificheGIDispatcherController> _logger;
        private readonly DBQueue.IQueueProvider _queueProvider;
        public NotificheGIDispatcherController(ILogger<NotificheGIDispatcherController> logger, DBQueue.IQueueProvider queueProvider)
        {
            _logger = logger;
            _queueProvider = queueProvider;
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ConflictResult), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult> Post([FromBody] Model.NotificaGI ngi)
        {
            try
            {
                _logger.LogInformation(0, "Add NotificaGI {0}", ngi.ProtocolloGI);

                
                string j = System.Text.Json.JsonSerializer.Serialize(ngi);
                

                // add to queue
                DBQueue.IPublisher pub = _queueProvider.GetQueuePublisher();
                Message message = await pub.EnqueueAsync("NotificaGI", j);

                _logger.LogInformation(0, "NotificaGI added to the queue");

                //return CreatedAtAction(nameof(Get), new { QueueId = id });
                return Ok(message.Header.MessageId);

            }
            catch (Exception ex)
            {

                _logger.LogError(0, ex, ex.Message);

                return Problem(
                    title: "An error has occurred",
                    detail: ex.Message,
                    statusCode: StatusCodes.Status500InternalServerError);


            }

        }

        private object Get()
        {
            throw new NotImplementedException();
        }
    }
}

