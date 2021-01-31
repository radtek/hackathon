using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace XPInc.Hackathon.Hosts.Api.Middlewares
{
    // Implemented as https://tools.ietf.org/html/rfc7807.
    public static class HttpOutput
    {
        public static string DefaultProblemType { get; } = "https://httpstatuses.com";

        public static ProblemDetails CreateProblemDetail(string title, int statusCode, string instance, string detail)
            => CreateProblemDetail(title, statusCode, instance, detail, null);

        public static ProblemDetails CreateProblemDetail(string title, int statusCode, Uri instance, string detail)
            => CreateProblemDetail(title, statusCode, instance, detail, null);

        public static ProblemDetails CreateProblemDetail(string title, int statusCode, string instance, string detail, Exception e)
        {
            if (!Uri.IsWellFormedUriString(instance, UriKind.Relative))
            {
                throw new ArgumentException("Problem detail with invalid relative URI.", nameof(instance));
            }

            return CreateProblemDetail(title, statusCode, new Uri(instance, UriKind.Relative), detail, e);
        }

        public static ProblemDetails CreateProblemDetail(string title, int statusCode, Uri instance, string detail, Exception e)
        {
            if (title == default)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (title == string.Empty)
            {
                throw new ArgumentException("Problem detail without a title.", nameof(title));
            }

            if (statusCode < 100 || statusCode > 599)
            {
                throw new ArgumentException("Invalid status code.", nameof(title));
            }

            if (instance == default)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (instance.IsAbsoluteUri)
            {
                throw new ArgumentException("Problem detail with invalid relative URI.", nameof(instance));
            }

            if (detail == default)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (detail == string.Empty)
            {
                throw new ArgumentException("Problem detail without a detail message.", nameof(title));
            }

            var problem = new ProblemDetails
            {
                Title = title,
                Type = $"{DefaultProblemType}/{statusCode}",
                Instance = instance.OriginalString,
                Status = statusCode,
                Detail = detail
            };

            if (e != default)
            {
                var validationException = e as ValidationException;

                if (validationException != null)
                {
                    problem.Extensions.Add("validation", validationException.ValidationResult);

                    // We need a better solution with error messaging (avoid implementation exposure).
                    if (e.InnerException != default)
                    {
                        problem.Extensions.Add("diagnostic", e.InnerException.Message);
                    }
                }
            }

            return problem;
        }
    }
}
