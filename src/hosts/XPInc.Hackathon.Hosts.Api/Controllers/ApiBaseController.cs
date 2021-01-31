using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XPInc.Hackathon.Hosts.Api.Configuration;

namespace XPInc.Hackathon.Hosts.Api.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}")]
    [Produces("application/json")]
    public abstract class ApiBaseController<TLogType> : ControllerBase
        where TLogType : class
    {
        protected readonly ILogger logger;
        protected readonly IMediator mediator;

        private readonly IOptions<HostsOptions> _options;

        protected ApiBaseController([FromServices] ILogger<TLogType> logger,
                                    [FromServices] IOptions<HostsOptions> options,
                                    [FromServices] IMediator mediator)
        {
            if (logger == default)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (mediator == default)
            {
                throw new ArgumentNullException(nameof(mediator));
            }

            this._options = options;

            this.logger = logger;
            this.mediator = mediator;
        }
    }
}
