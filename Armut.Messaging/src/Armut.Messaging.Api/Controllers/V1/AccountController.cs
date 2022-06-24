using Armut.Messaging.Application.DTOs;
using Armut.Messaging.Application.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace Armut.Messaging.Api.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterAsync([FromBody] AuthRequest request, CancellationToken cancellationToken)
        {            
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"{request.Username} is already registered.", DateTime.Now);
                return BadRequest();
            }

            await _accountService.RegisterAsync(request.Username, request.Password, cancellationToken);

            _logger.LogInformation($"{request.Username} is succesfuly registered.", DateTime.Now);
            return Ok();
        }
                
        [HttpGet]
        public async Task<ActionResult<AuthResponse>> AuthenticateAsync([FromQuery] AuthRequest request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("User logging...", DateTime.Now);
            if (!ModelState.IsValid)
            {
                _logger.LogInformation($"You have entered an invalid username or password.", DateTime.Now);
                return BadRequest();
            }

            var result = await _accountService.AuthenticateAsync(request.Username, request.Password, cancellationToken);

            _logger.LogInformation($"{request.Username} is succesfuly logged in.", DateTime.Now);
            return Ok(result);
        }
    }
}
