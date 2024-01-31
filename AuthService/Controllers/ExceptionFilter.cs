using System.Net;
using AuthService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AuthService.Controllers;

public class ExceptionFilter: IActionFilter, IOrderedFilter
{
    public int Order => int.MaxValue - 10; // Always execute last in pipeline

    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is AuthServiceException exc)
        {
            context.Result = new BadRequestObjectResult(new {exc.Message});
            context.ExceptionHandled = true;
        }
        else if (context.Exception is not null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            _logger.LogError(context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }

    
}