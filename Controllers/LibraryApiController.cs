using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace LibraryManagementSystem.Controllers;

[ApiController]
[Route("api/library")]
public class LibraryApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public LibraryApiController(
    ApplicationDbContext context,
    IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private bool HasValidApiKey()
    {
        if (!Request.Headers.TryGetValue("X-API-Key", out var providedKey))
        {
            return false;
        }

        var expectedKey = _configuration["LibraryApi:ApiKey"];

        return !string.IsNullOrWhiteSpace(expectedKey) &&
               providedKey == expectedKey;
    }



    // GET: /api/library/items
    [HttpGet("items")]
    public async Task<IActionResult> GetItems()
    {
        if (!HasValidApiKey())
        {
            return Unauthorized(new
            {
                message = "A valid API key is required."
            });
        }
        var items = await _context.Items
            .Include(i => i.LibraryBranch)
            .OrderBy(i => i.Name)
            .Select(i => new
            {
                i.Id,
                i.Name,
                i.Description,
                i.LibraryCode,
                Status = i.Status.ToString(),
                Branch = i.LibraryBranch != null
                    ? i.LibraryBranch.Name
                    : "Not Assigned",
                Category = i.GetType().Name
            })
            .ToListAsync();

        return Ok(items);
    }


    // GET: /api/library/items/5
    [HttpGet("items/{id:int}")]
    public async Task<IActionResult> GetItem(int id)
    {
        if (!HasValidApiKey())
        {
            return Unauthorized(new
            {
                message = "A valid API key is required."
            });
        }
        var item = await _context.Items
            .Include(i => i.LibraryBranch)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (item == null)
        {
            return NotFound(new
            {
                message = "Library item not found."
            });
        }

        return Ok(new
        {
            item.Id,
            item.Name,
            item.Description,
            item.LibraryCode,
            Status = item.Status.ToString(),
            Branch = item.LibraryBranch?.Name ?? "Not Assigned",
            Category = item.GetType().Name
        });
    }

    // GET: /api/library/categories
    [HttpGet("categories")]
    public IActionResult GetCategories()
    {
        if (!HasValidApiKey())
        {
            return Unauthorized(new
            {
                message = "A valid API key is required."
            });
        }
        return Ok(new[]
        {
            "Book",
            "Music",
            "Toy"
        });
    }

    // GET: /api/library/status
    [HttpGet("status")]
    public IActionResult GetOperatingStatus()
    {
        if (!HasValidApiKey())
        {
            return Unauthorized(new
            {
                message = "A valid API key is required."
            });
        }
        return Ok(new
        {
            service = "Library Catalogue API",
            status = "Operational",
            access = "Read Only",
            timestamp = DateTime.Now
        });
    }


}