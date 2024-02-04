using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BudgetModel.Enums;
using BudgetWebApi.Dto;

namespace BudgetWebApi.Controllers;

[ApiController]
[Authorize]
[Route("budgets")]
public class CategoryController : ControllerBase
{
    private readonly ILogger<CategoryController> _logger;
    private readonly CategoryService _categoryService;

    public CategoryController(ILogger<CategoryController> logger, CategoryService categoryService)
    {
        _logger = logger;
        _categoryService = categoryService;
    }

    [HttpPost]
    [Route("{budgetId}/categories/add")]
    public IActionResult Add([FromBody] Dto.CategoryAdd payload, string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(new Dto.CategoryDto(_categoryService.AddCategory(budgetId,
                                                                     payload.CategoryId,
                                                                     requestingUser!,
                                                                     payload.Description,
                                                                     payload.DefaultType)));
    }

    [HttpGet]
    [Route("{budgetId}/categories")]
    public IActionResult GetAll(string budgetId)
    {
        string? requestingUser = User.Identity?.Name;
        return Ok(_categoryService.GetAllCategories(budgetId, requestingUser!).Select(c => new Dto.CategoryDto(c)));
    }

    [HttpGet]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult GetOne(string budgetId, string catId)
    {
        string? requestingUser = User.Identity?.Name; 
        return Ok(new Dto.CategoryDto(_categoryService.GetCategory(budgetId, catId, requestingUser!)));
    }

    [HttpPut]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult Update([FromBody] Dto.CategoryUpdate payload, string budgetId, string catId)
    {
        string? requestingUser = User.Identity?.Name;
        var c = _categoryService.GetCategory(budgetId, catId, requestingUser!);
        c.Description = payload.Description;
        c.DefaultType = (TransactionType)payload.DefaultType;
        
        return Ok(new Dto.CategoryDto(_categoryService.UpdateCategory(budgetId, catId, requestingUser!, c)));
    }


    [HttpGet]
    [Route("{budgetId}/categories/{catId}/changeStatus")]
    public IActionResult ChangeStatus(string budgetId, string catId)
    {
        string? requestingUser = User.Identity?.Name;
        _categoryService.ChangeStatus(budgetId, catId, requestingUser!);
        return Ok();
    }

    [HttpDelete]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult Delete(string budgetId, string catId)
    {
        string? requestingUser = User.Identity?.Name;
        _categoryService.DeleteCategory(budgetId, catId, requestingUser!);
        return Ok();
    }
    
}