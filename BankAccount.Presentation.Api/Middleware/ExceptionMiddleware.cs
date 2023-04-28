using FluentValidation;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using Newtonsoft.Json;

namespace BankAccount.Presentation.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext? context)
        {
            try
            {
                if (context == null) return;

                _logger.LogInformation("{Username} request host {host}",
                    context.User?.Identity?.Name ?? "Anonymous",
                    context.Request?.GetDisplayUrl());

                await _next(context!);
            }
            catch (Exception e)
            {
                _logger.LogError($"Error ExceptionMiddleware: {e.Message}");
                await HandleException(context!, e);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            var statusCode = GetStatusCode(exception);

            var response = new
            {
                Title = GetTitle(exception),
                StatusCode = statusCode,
#if DEBUG
                Detail = exception.Message,
#endif
                errors = GetErrors(exception)
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }

        private object GetErrors(Exception exception)
        {
            List<dynamic> errors = new();
            switch (exception)
            {
                case ValidationException validationException:
                    errors.AddRange(validationException.Errors
                        .Select(x => new
                        {
                            x.PropertyName,
                            x.ErrorMessage,
                            x.ErrorCode
                        }).ToList());
                    break;
                case ArgumentException argumentException:
                    errors.Add(new
                    {
                        ErrorMessage = argumentException.Message,
                        ErrorCode = HttpStatusCode.BadRequest
                    });
                    break;
                case OperationCanceledException operationCanceledException:
                    errors.Add(
                        new
                        {
                            ErrorMessage = operationCanceledException.Message,
                            ErrorCode = HttpStatusCode.BadRequest
                        });
                    break;
                case InvalidOperationException invalidOperationException:
                    errors.Add(
                        new
                        {
                            ErrorMessage = invalidOperationException.Message,
                            ErrorCode = HttpStatusCode.BadRequest
                        });
                    break;
                default:
                    errors.Add(new
                    {
                        ErrorMessage = "Internal Server Error"
                    });
                    break;
            }

            return errors;
        }

        private int GetStatusCode(Exception exception)
        {
            return exception switch
            {
                ValidationException => StatusCodes.Status422UnprocessableEntity,
                ArgumentException => StatusCodes.Status400BadRequest,
                InvalidOperationException => StatusCodes.Status500InternalServerError,
                OperationCanceledException => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }

        private string GetTitle(Exception exception)
        {
            return exception switch
            {
                ApplicationException applicationException => applicationException.Message,
                ValidationException => "Validation Failed",
                ArgumentException => "Values is not correct",
                InvalidOperationException => exception.Message,
                OperationCanceledException => exception.Message,
                _ => "Server Error"
            };
        }
    }
}
