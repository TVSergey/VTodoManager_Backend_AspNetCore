using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Vtodo.Entities.Exceptions;

namespace Vtodo.Web.Utils
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        
        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (CustomException e)
            {
                await HandleException(httpContext, e);
            }
        }

        private async Task HandleException(HttpContext httpContext, CustomException customException)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)customException.Code;
            await httpContext.Response.WriteAsync(customException.Message);
        }
    }
}