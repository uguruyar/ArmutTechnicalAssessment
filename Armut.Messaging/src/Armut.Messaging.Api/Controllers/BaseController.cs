using Microsoft.AspNetCore.Mvc;

namespace Armut.Messaging.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        public string UserId
        {
            get
            {
                return HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Sid)?.Value ?? string.Empty;
            }
        }

        public string UserName
        {
            get
            {
                return HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value ?? string.Empty;
            }
        }
    }
}
