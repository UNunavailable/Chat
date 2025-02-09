using MessageService.Models;
using MessageService.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly MessageRepository _repository;
        private readonly IHubContext<MessageHub> _hubContext;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(
            MessageRepository repository,
            IHubContext<MessageHub> hubContext,
            ILogger<MessagesController> logger)
        {
            _repository = repository;
            _hubContext = hubContext;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> PostMessage([FromBody] MessageRequest request)
        {
            try
            {
                var message = new Message
                {
                    Content = request.Content,
                    Timestamp = DateTime.UtcNow,
                    ClientNumber = request.ClientNumber
                };

                await _repository.AddMessageAsync(message);
                await _hubContext.Clients.All.SendAsync("ReceiveMessage",
                    message.Content, message.Timestamp, message.ClientNumber);

                _logger.LogInformation("Message processed: {Id}", message.Id);
                return Ok(new { message.Id });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing message");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMessages([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            try
            {
                var messages = await _repository.GetMessagesAsync(start, end);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving messages");
                return StatusCode(500, "Internal server error");
            }
        }

        public class MessageRequest
        {
            public string Content { get; set; }
            public int ClientNumber { get; set; }
        }
    }

}
