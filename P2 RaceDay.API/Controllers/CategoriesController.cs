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
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateCategory(
        CategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(
                "Name and description are required.");
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description.Trim()
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetCategory),
            new { id = category.CategoryID },
            category);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateCategory(
        int id, CategoryDto dto)
    {
        var category = await _context.Categories
            .FindAsync(id);

        if (category == null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(dto.Name) ||
            string.IsNullOrWhiteSpace(dto.Description))
        {
            return BadRequest(
                "Name and description are required.");
        }

        category.Name = dto.Name.Trim();
        category.Description = dto.Description.Trim();

        await _context.SaveChangesAsync();

        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _context.Categories
            .FindAsync(id);

        if (category == null)
            return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return NoContent();
    }






}
