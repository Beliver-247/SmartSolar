/*
 * Purpose: Global exception handling middleware.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var statusCode = StatusCodes.Status500InternalServerError;
            var message = "An internal server error occurred.";

            if (exception is BusinessRuleException)
            {
                statusCode = StatusCodes.Status409Conflict;
                message = exception.Message;
            }
            else if (exception is NotFoundException)
            {
                statusCode = StatusCodes.Status404NotFound;
                message = exception.Message;
            }
            else if (exception is ArgumentException)
            {
                statusCode = StatusCodes.Status400BadRequest;
                message = exception.Message;
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = StatusCodes.Status403Forbidden;
                message = exception.Message;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var result = JsonSerializer.Serialize(new
            {
                status = statusCode,
                message = message,
                timestamp = DateTimeOffset.UtcNow,
                path = context.Request.Path
            });

            return context.Response.WriteAsync(result);
        }
    }
}
