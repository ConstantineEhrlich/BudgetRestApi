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
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(new Dto.CategoryDto(_categoryService.AddCategory(budgetId, payload.CategoryId, requestingUser,
            payload.Description)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpGet]
    [Route("{budgetId}/categories")]
    public IActionResult GetAll(string budgetId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(_categoryService.GetAllCategories(budgetId, requestingUser).Select(c => new Dto.CategoryDto(c)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpGet]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult GetOne(string budgetId, string catId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            return Ok(new Dto.CategoryDto(_categoryService.GetCategory(budgetId, catId, requestingUser)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpPut]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult Update([FromBody] Dto.CategoryUpdate payload, string budgetId, string catId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            var c = _categoryService.GetCategory(budgetId, catId, requestingUser);
            c.Description = payload.Description;
            c.DefaultType = (TransactionType)payload.DefaultType;
            
            return Ok(new Dto.CategoryDto(_categoryService.UpdateCategory(budgetId, catId, requestingUser, c)));
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }

    [HttpDelete]
    [Route("{budgetId}/categories/{catId}")]
    public IActionResult Delete(string budgetId, string catId)
    {
        string requestingUser = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        try
        {
            _categoryService.DeleteCategory(budgetId, catId, requestingUser);
            return Ok();
        }
        catch (ArgumentException e)
        {
            return BadRequest(new { Message = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode(500, new { Message = "Internal server error" });
        }
    }
    
}