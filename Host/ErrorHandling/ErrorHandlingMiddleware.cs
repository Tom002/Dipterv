using Dipterv.Shared.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Templates.TodoApp.Host.ErrorHandling
{
    public class ErrorHandlingMiddleware
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IJsonHelper _jsonHelper;

        public ErrorHandlingMiddleware(
        RequestDelegate next,
        ProblemDetailsFactory problemDetailsFactory,
        IJsonHelper jsonHelper)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _jsonHelper = jsonHelper;
        }

        /// <summary>
        /// Invoke middleware.
        /// </summary>
        public async Task InvokeAsync(HttpContext httpContext)
        {
            Exception exception = httpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

            int statusCode = (int)HttpStatusCode.InternalServerError;

            string title = "An error occured";
            string detail = null!;
            string instance = httpContext.Request.Path;

            ModelStateDictionary validationErrors = new();

            if (exception != null)
            {
                detail = exception.Message;

                if (exception is BusinessException businessException)
                {
                    if (!string.IsNullOrEmpty(businessException.Title))
                        title = businessException.Title;

                    if (!string.IsNullOrEmpty(businessException.Details))
                        detail = businessException.Details;

                    statusCode = (int)HttpStatusCode.BadRequest;
                }

                if (exception is ValidationException ve)
                {
                    title = string.IsNullOrEmpty(ve.Title) ? "Validation error!" : ve.Title;

                    foreach (ValidationError item in ve.Errors)
                        validationErrors.AddModelError(item.Key, item.Error);
                }

                if (exception is UnauthorizedAccessException)
                {
                    title = "Unauthorized";
                    statusCode = (int)HttpStatusCode.Unauthorized;
                }

                if (exception is OperationCanceledException)
                {
                    statusCode = 499;
                }

                if (exception is TimeoutException)
                {
                    title = "Gateway timeout";
                    statusCode = (int)HttpStatusCode.GatewayTimeout;
                }
            }

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";

            Microsoft.AspNetCore.Mvc.ProblemDetails problemDetails = statusCode == (int)HttpStatusCode.BadRequest
                ? _problemDetailsFactory.CreateValidationProblemDetails(httpContext, validationErrors, statusCode, title, detail: detail, instance: instance)
                : _problemDetailsFactory.CreateProblemDetails(httpContext, statusCode, title, detail: detail, instance: instance);

            await WriteResponseAsync(httpContext.Response, _jsonHelper.Serialize(problemDetails));
        }

        private async Task WriteResponseAsync(HttpResponse response, IHtmlContent content)
        {
            using MemoryStream mStream = new();
            using StreamWriter writer = new(mStream);

            content.WriteTo(writer, HtmlEncoder.Default);
            await writer.FlushAsync();
            await response.Body.WriteAsync(mStream.ToArray());
        }
    }
}
