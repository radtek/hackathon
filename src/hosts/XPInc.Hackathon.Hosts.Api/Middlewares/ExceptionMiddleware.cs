using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Threading.Tasks;
using XPInc.Hackathon.Framework.Serialization;

namespace XPInc.Hackathon.Hosts.Api.Middlewares
{
    public class ExceptionMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware([FromServices] ILogger<ExceptionMiddleware> logger)
        {
            if (logger == default)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._logger = logger;
        }

        public static string ContentType { get; } = "application/json";

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ArgumentNullException e)
            {
                this._logger.LogError(e, "A method call has a null pointer value.");

                await this.CreateProblemOutputAsync("Invalid reference in an operation.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (ArgumentException e)
            {
                this._logger.LogError(e, "Method call with invalid paremeter.");

                await this.CreateProblemOutputAsync("Invalid parameter in an operation.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (AggregateException e)
            {
                this._logger.LogError(e, "Some set of instructions were faulty.");

                await this.CreateProblemOutputAsync("Error processing a set of instructions.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (FormatException e)
            {
                this._logger.LogError(e, "Format not compatible.");

                await this.CreateProblemOutputAsync("Error when converting/formating data.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (ValidationException e)
            {
                this._logger.LogError(e, "Validation exception occurred.");

                await this.CreateProblemOutputAsync("Client error(s) found.", StatusCodes.Status400BadRequest, context, e);
            }
            catch (InvalidOperationException e)
            {
                this._logger.LogError(e, "An invalid operation has occurred.");

                await this.CreateProblemOutputAsync("An invalid operation has occurred.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (InvalidCastException e)
            {
                this._logger.LogError(e, "An invalid casting has occurred.");

                await this.CreateProblemOutputAsync("An invalid internal conversion has occurred.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (HttpRequestException e)
            {
                this._logger.LogError(e, "HTTP operation not successful.");

                await this.CreateProblemOutputAsync("An invalid HTTP operation has occurred.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (NotImplementedException e)
            {
                this._logger.LogError(e, "A method without implementation has been called.");

                await this.CreateProblemOutputAsync("Operation not defined.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (TimeoutException e)
            {
                this._logger.LogError(e, "Operation taking longer than normal.");

                await this.CreateProblemOutputAsync("Operation taking longer than normal.", StatusCodes.Status500InternalServerError, context, e);
            }
            catch (OperationCanceledException e)
            {
                this._logger.LogError("Some server tasks have been cancelled. Origin (method): {0}.", e.TargetSite.Name);
            }
        }

        private async Task CreateProblemOutputAsync(string title, int statusCode, HttpContext context, Exception e)
        {
            var serializer = (ICacheSerializerAsync<string>)context.RequestServices.GetService(typeof(ICacheSerializerAsync<string>));
            var problem = HttpOutput.CreateProblemDetail(title, statusCode, context.Request.Path.Value, e.Message, e);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = ContentType;

            await context.Response.WriteAsync(await serializer.SerializeAsync(problem), context.RequestAborted);
        }
    }
}
