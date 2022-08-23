using Microsoft.AspNetCore.Mvc;

namespace Armut.Messaging.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public string UserId => HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Sid)?.Value ?? string.Empty;

        public string UserName => HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? string.Empty;
    }
}
