using BudgetServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BudgetModel.Enums;
using BudgetModel.Models;

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
    public async Task<ActionResult<Category>> Add([FromBody] Dto.CategoryAdd payload, string budgetId)
    {
        _logger.LogInformation("Adding category to budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        return await _categoryService.AddCategory(budgetId,
                                                     payload.CategoryId!,
                                                     requestingUser!,
                                                     payload.Description,
                                                     payload.DefaultType);
    }

    [HttpGet]
    [Route("{budgetId}/categories")]
    public async Task<ActionResult<List<Category>>> GetAll(string budgetId)
    {
        _logger.LogInformation("Getting categories for budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        return await _categoryService.GetAllCategories(budgetId, requestingUser!);
    }

    [HttpGet]
    [Route("{budgetId}/categories/{catId}")]
    public async Task<ActionResult<Category>> GetOne(string budgetId, string catId)
    {
        _logger.LogInformation("Getting category from budget {}", budgetId);
        string? requestingUser = User.Identity?.Name; 
        return await _categoryService.GetCategory(budgetId, catId, requestingUser!);
    }

    [HttpPut]
    [Route("{budgetId}/categories/{catId}")]
    public async Task<ActionResult<Category>> Update([FromBody] Dto.CategoryUpdate payload, string budgetId, string catId)
    {
        _logger.LogInformation("Updating category from budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        Category c = await _categoryService.GetCategory(budgetId, catId, requestingUser!);
        c.Description = payload.Description;
        c.DefaultType = (TransactionType)payload.DefaultType;
        
        return await _categoryService.UpdateCategory(budgetId, catId, requestingUser!, c);
    }


    [HttpGet]
    [Route("{budgetId}/categories/{catId}/changeStatus")]
    public async Task<ActionResult> ChangeStatus(string budgetId, string catId)
    {
        _logger.LogInformation("Changing status of the category from budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        await _categoryService.ChangeStatus(budgetId, catId, requestingUser!);
        return Ok();
    }

    [HttpDelete]
    [Route("{budgetId}/categories/{catId}")]
    public async Task<ActionResult> Delete(string budgetId, string catId)
    {
        _logger.LogInformation("Deleting category from budget {}", budgetId);
        string? requestingUser = User.Identity?.Name;
        await _categoryService.DeleteCategory(budgetId, catId, requestingUser!);
        return Ok();
    }
    
}