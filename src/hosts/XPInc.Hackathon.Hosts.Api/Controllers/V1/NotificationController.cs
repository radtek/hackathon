using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XPInc.Hackathon.Hosts.Api.Configuration;

namespace XPInc.Hackathon.Hosts.Api.Controllers.V1
{
    [Authorize]
    [ApiVersion("1")]
    [ApiExplorerSettings(GroupName = "Version 1")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status408RequestTimeout)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status415UnsupportedMediaType)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public class NotificationController : ApiBaseController<NotificationController>
    {
        public NotificationController([FromServices] ILogger<NotificationController> logger,
                                      [FromServices] IOptions<HostsOptions> options,
                                      [FromServices] IMediator mediator)
            : base(logger, options, mediator)
        { }
    }
}
