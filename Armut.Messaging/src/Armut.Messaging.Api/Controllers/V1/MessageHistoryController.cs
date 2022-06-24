using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Armut.Messaging.Domain.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Armut.Messaging.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class MessageHistoryController : BaseController
    {
        private readonly IMessageHistoryService _messageHistoryService;

        public MessageHistoryController(IMessageHistoryService messageHistoryService)
        {
            _messageHistoryService = messageHistoryService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserMessage>>> GetAsync([FromQuery]MessageHistoryRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result = await _messageHistoryService.GetMessageHistoryAsync(UserName, request.TargetUserName, cancellationToken);

            if (result == null || !result.Any())
            {
                return NoContent();
            }

            return Ok(result);
        }
    }   
}

