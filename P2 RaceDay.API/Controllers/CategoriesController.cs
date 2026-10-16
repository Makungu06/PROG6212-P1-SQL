using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using P2_RaceDay.API.Data;
using P2_RaceDay.API.Models;    

namespace P2_RaceDay.API.Controllers;

public class CategoryDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

[ApiController]
[Route("api/categories")]


public class CategoriesController : ControllerBase  
{
    private readonly ApplicationDbContext _context;

    public CategoriesController(ApplicationDbContext context)
    {
        _context = context;
    }
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategories()
    {
        return Ok(await _context.Categories.ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]

    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _context.Categories
            .FindAsync(id);

        return category == null
            ? NotFound()
            : Ok(category);
    }






}
