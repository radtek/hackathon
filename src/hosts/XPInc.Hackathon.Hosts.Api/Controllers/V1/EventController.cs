using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XPInc.Hackathon.Core.Application.Models.Request;
using XPInc.Hackathon.Core.Domain.Commands;
using XPInc.Hackathon.Core.Domain.Repositories;
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
        private readonly IEventRepository _eventRepository;

        public EventController([FromServices] ILogger<EventController> logger,
                               [FromServices] IOptions<HostsOptions> options,
                               [FromServices] IMediator mediator,
                               [FromServices] IEventRepository eventRepository)
            : base(logger, options, mediator)
        {
            _eventRepository = eventRepository ?? throw new ArgumentNullException();
        }

        /// <summary>
        /// Get a list of events.
        /// </summary>
        /// <remarks>.</remarks>
        /// <param name="teamId">Team ID.</param>
        /// <param name="cancellationToken">Cancellation token (emitted automatically when the request is aborted by the client).</param>
        /// <returns>A list of events.</returns>
        /// <response code="200">Events found.</response>
        /// <response code="400">When client errors occur.</response>
        /// <response code="404">Resource not found.</response>
        /// <response code="415">Requested format is not supported by the server.</response>
        /// <response code="500">When server errors occur.</response>
        [HttpGet("{teamId}/events")]
        [MapToApiVersion("1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ResponseCache(NoStore = true, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> GetEventsAsync(string teamId, CancellationToken cancellationToken = default)
        {
            var response = await _eventRepository.GetEventsAsync(teamId, cancellationToken).ConfigureAwait(false);

            return Ok(response);
        }

        /// <summary>
        /// Get an event.
        /// </summary>
        /// <remarks>.</remarks>
        /// <param name="teamId">Team ID.</param>
        /// <param name="id">Event ID.</param>
        /// <param name="cancellationToken">Cancellation token (emitted automatically when the request is aborted by the client).</param>
        /// <returns>The event.</returns>
        /// <response code="200">Event found.</response>
        /// <response code="400">When client errors occur.</response>
        /// <response code="404">Resource not found.</response>
        /// <response code="415">Requested format is not supported by the server.</response>
        /// <response code="500">When server errors occur.</response>
        [HttpGet("{teamId}/events/{id}")]
        [MapToApiVersion("1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ResponseCache(NoStore = true, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> GetEventAsync(string teamId, string id, CancellationToken cancellationToken = default)
        {
            var response = await _eventRepository.GetEventAsync(teamId, id, cancellationToken).ConfigureAwait(false);

            return Ok(response);
        }

        /// <summary>
        /// Treat an event.
        /// </summary>
        /// <remarks>.</remarks>
        /// <param name="teamId">Team ID.</param>
        /// <param name="id">Event ID.</param>
        /// <param name="request">Request parameters.</param>
        /// <param name="cancellationToken">Cancellation token (emitted automatically when the request is aborted by the client).</param>
        /// <returns>The event.</returns>
        /// <response code="200">Event found.</response>
        /// <response code="400">When client errors occur.</response>
        /// <response code="404">Resource not found.</response>
        /// <response code="415">Requested format is not supported by the server.</response>
        /// <response code="500">When server errors occur.</response>
        [HttpPatch("{teamId}/events/{id}")]
        [MapToApiVersion("1")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ResponseCache(NoStore = true, VaryByHeader = "User-Agent")]
        public async Task<IActionResult> TreatEventAsync(string teamId, string id, TreatEventRequest request, CancellationToken cancellationToken = default)
        {
            var @event = await _eventRepository.GetEventAsync(teamId, id, cancellationToken).ConfigureAwait(false);

            switch (request.Type)
            {
                case TreatEventRequestType.Ack:
                    @event.Ack(new AckEventCommand
                    {
                        Username = request.Username
                    });
                    break;
                case TreatEventRequestType.Close:
                    @event.Resolve(new ResolveEventCommand
                    {
                        Username = request.Username,
                        Message = request.Message
                    });
                    break;
                default:
                    throw new KeyNotFoundException();
            }

            return NoContent();
        }
    }
}
