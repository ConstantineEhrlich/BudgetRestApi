using System.Net;
using BudgetServices;
using BudgetServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BudgetWebApi.Controllers;

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
        if (context.Exception is BudgetServiceException exc)
        {
            context.Result = new BadRequestObjectResult(new {exc.Message});
            context.ExceptionHandled = true;
        }
        else if (context.Exception is not null)
        {
            context.Result = new StatusCodeResult(StatusCodes.Status500InternalServerError);
            _logger.LogError("Error occured:{}", context.Exception.Message);
            context.ExceptionHandled = true;
        }
    }

    
}