using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Armut.Messaging.Api.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class BlockUserController : ControllerBase
    {
        private readonly IUserService _userService;

        public BlockUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public async Task<ActionResult> BlockUserAsync([FromBody] BlockUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _userService.BlockUserAsync(request.TargetUserName, cancellationToken);

            return Ok();
        }
    }
}