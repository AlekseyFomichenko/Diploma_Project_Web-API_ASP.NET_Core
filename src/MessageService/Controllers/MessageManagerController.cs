using System.Security.Claims;
using MessageService.Interfaces;
using MessageService.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MessageManagerController : ControllerBase
    {
        private readonly IMessageRepo _messageRepo;

        public MessageManagerController(IMessageRepo messageRepo)
        {
            _messageRepo = messageRepo;
        }

        [HttpGet("GetMessages")]
        public IActionResult GetMessages()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var receiverId))
                return Unauthorized();
            var messages = _messageRepo.GetAllMessages(receiverId);
            return Ok(messages);
        }

        [HttpPost("SendMessage")]
        public IActionResult SendMessage([FromBody] SendMessageRequest request)
        {
            if (request?.Text == null)
                return BadRequest("Text required");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var senderId))
                return Unauthorized();
            var result = _messageRepo.SendMessage(request.Text, senderId, request.ReceiverId);
            return result > 0 ? Ok("Successfully!") : StatusCode(500);
        }
    }
}
