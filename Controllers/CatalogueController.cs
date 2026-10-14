using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;

[AllowAnonymous]

public class CatalogueController : Controller
{
    private readonly ApplicationDbContext _context;

    public CatalogueController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchString)
    {
        var items = _context.Items
    .Include(i => i.LibraryBranch)
    .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchString))
        {
            items = items.Where(i =>
                i.Name.Contains(searchString) ||
                i.Description.Contains(searchString) ||
                i.LibraryCode.Contains(searchString) ||

                (i is Book &&
                    (((Book)i).Author.Contains(searchString) ||
                     ((Book)i).Genre.Contains(searchString))) ||

                (i is Music &&
                    ((Music)i).Artist.Contains(searchString)) ||

                (i is Toy &&
                    ((Toy)i).Type.Contains(searchString))
            );
        }

        var results = await items
            .OrderBy(i => i.Name)
            .ToListAsync();

        ViewData["CurrentSearch"] = searchString;

        return View(results);
    }
}