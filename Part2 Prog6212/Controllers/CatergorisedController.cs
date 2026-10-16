using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Part2_Prog6212.Data;
using Part2_Prog6212.DTOs;  
using Part2_Prog6212.Models;
using System.ComponentModel.DataAnnotations;    


namespace Part2_Prog6212.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly ApplicationDbContext _db;

    public CategoriesController(ApplicationDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _db.Categories
            .AsNoTracking()
            .ToListAsync());
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _db.Categories.FindAsync(id);

        return category == null
            ? NotFound()
            : Ok(category);
    }

    [HttpPost]
    [Authorize(Roles = "Organiser")]
    public async Task<IActionResult> Create(
        CategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            CategoryType = request.CategoryType,
            Distance = request.Distance,
            MinAge = request.MinAge,
            MaxAge = request.MaxAge
        };

        _db.Categories.Add(category);
        await _db.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = category.CategoryId },
            category); 
    }

}
