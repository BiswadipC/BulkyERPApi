using Domain.ChatBuilder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.ChatBuilder;

namespace WebApiMain.Controllers
{
    [Route("api/chat")]
    [ApiController]

    public class ChatController : ControllerBase
    {
        private readonly IAIService _aiService;

        public ChatController(IAIService aIService)
        {
            _aiService = aIService;
        } // constructor...

        [HttpPost]
        public async Task<IActionResult> Chat(ChatRequest request)
        {
            string str = await _aiService.AskAsync(request.Message);
            return Ok(new ChatResponse()
            {
                Reply = str
            });
        } // Chat...
    } // class...
}
