using FormRecognizerApi.SignalR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormRecognizerApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PushMessageController
    {
        //IHubContext<PushHub, ISignalRHubService> _pushHubContext;
        private readonly IHubContext<PushHub> _hubContext;

        public PushMessageController(
            IHubContext<PushHub> hubContext)
        {
            _hubContext = hubContext;
        }


        [HttpPost("send-message")]
        public async Task<IActionResult> SendAsync(string recipientUserId, string fromUserId, string message)
        {
            try
            {
                await _hubContext.Clients.All.SendAsync(recipientUserId, fromUserId, message);
                return new OkObjectResult(new { success = true, error = "" });
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new { success = true, error = ex.Message });
            }
        }
    }
}
