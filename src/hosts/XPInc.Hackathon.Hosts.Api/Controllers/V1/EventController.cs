using System.Threading;
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
    public class EventController : ApiBaseController<EventController>
    {
        public EventController([FromServices] ILogger<EventController> logger,
                               [FromServices] IOptions<HostsOptions> options,
                               [FromServices] IMediator mediator)
            : base(logger, options, mediator)
        { }

        /// <summary>
        /// Authenticates a client.
        /// </summary>
        /// <remarks>Creates a JWT token with client's credential (client secret).</remarks>
        /// <param name="request">Client secret.</param>
        /// <param name="cancellationToken">Cancellation token (emitted automatically when the request is aborted by the client).</param>
        /// <returns>A time sensitive JWT token.</returns>
        /// <response code="200">Token created.</response>
        /// <response code="400">When client errors occur.</response>
        /// <response code="404">Resource not found.</response>
        /// <response code="415">Requested format is not supported by the server.</response>
        /// <response code="500">When server errors occur.</response>
        [HttpGet("events")]
        [MapToApiVersion("1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ResponseCache(NoStore = true, VaryByHeader = "User-Agent")]
        public IActionResult FooAsync(CancellationToken cancellationToken = default)
        {
            return default;
        }
    }
}
