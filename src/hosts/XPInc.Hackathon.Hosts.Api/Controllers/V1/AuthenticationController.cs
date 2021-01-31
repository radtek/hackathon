using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Claims;
using System.Threading;
using XPInc.Hackathon.Core.Application.Models;
using XPInc.Hackathon.Core.Application.Services;
using XPInc.Hackathon.Hosts.Api.Configuration;
using XPInc.Hackathon.Hosts.Api.Middlewares;

namespace XPInc.Hackathon.Hosts.Api.Controllers.V1
{
    [ApiController]
    [ApiExplorerSettings(GroupName = "Version 1")]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}")]
    [Consumes("application/json")]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private const string ClientSecret = "coe";

        private readonly ILogger<AuthenticationController> _logger;
        private readonly HostsOptions _options;
        private readonly IJwtAuthentication _authenticator;

        public AuthenticationController([FromServices] ILogger<AuthenticationController> logger,
                                        [FromServices] IOptions<HostsOptions> options,
                                        [FromServices] IJwtAuthentication authenticator)
        {
            if (logger == default)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (authenticator == default)
            {
                throw new ArgumentNullException(nameof(authenticator));
            }

            this._logger = logger;
            this._options = options.Value;
            this._authenticator = authenticator;
        }

        // Field as https://tools.ietf.org/html/rfc7519#section-4.1.7
        public static string ClientSecretKey { get; } = "jti";

        // Field as https://tools.ietf.org/html/rfc8693#section-4.3
        public static string ClientIdKey { get; } = "client_id";

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
        [HttpPost("token")]
        [MapToApiVersion("1")]
        [ProducesResponseType(typeof(AuthenticationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        [ResponseCache(NoStore = true, VaryByHeader = "User-Agent")]
        public IActionResult Authenticate([FromBody] AuthenticationRequest request,
                                          CancellationToken cancellationToken = default)
        {
            var options = this._options.Api.Authentication;

            if (!options.IsExpiringEnabled)
            {
                throw new InvalidOperationException("Could not create a JWT token without expiring information.");
            }

            if (!string.Equals(request.ClientSecret, ClientSecret, StringComparison.InvariantCultureIgnoreCase))
            {
                this._logger.LogWarning("Client secret \"{secret}\" was not found.", request.ClientSecret);

                return this.NotFound(
                    HttpOutput.CreateProblemDetail(
                        "Client secret not found.",
                        StatusCodes.Status404NotFound,
                        this.HttpContext?.Request.Path.Value ?? "unit-test",
                        "Your client secret was not found. Please, try again. If it persists, contact us and request a new one."
                    )
                );
            }

            var claims = new[]
            {
                new Claim(ClientIdKey, this.HttpContext?.Connection.RemoteIpAddress.ToString() ?? string.Empty),
                new Claim(ClientSecretKey, request.ClientSecret)
            };

            var token = this._authenticator.CreateToken(claims, options.TokenDuration.Value);

            this._logger.LogInformation("JWT token generated for \"{secret}\".", request.ClientSecret);

            return this.Ok(new AuthenticationResponse(token, options.TokenDuration.Value));
        }
    }
}
