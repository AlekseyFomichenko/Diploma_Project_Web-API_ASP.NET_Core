using MessageService.Interfaces;
using MessageService.Models.Dto;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessageManagerController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;
        public MessageManagerController(IMessageRepo messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet]
        [Route("GetMessages")]
        public IActionResult GetAllMessages(string userName)
        {
            var messages = _messageRepo.GetAllMessages(userName);
            return Ok(messages);
        }

        [HttpPost]
        [Route("SendMessage")]
        public IActionResult SendMessage(MessageDTO message)
        {
            if (message.SenderMail != null && message.ReceiverMail != null && message.Text != null)
            {
                var messageResult = _messageRepo.SendMessage(
                    message.Text,
                    message.SenderMail,
                    message.ReceiverMail
                );
                if (messageResult > 0)
                    return Ok("Successfully!");
            }
            return NotFound("Sender or Receiver not fount");
        }
    }
}