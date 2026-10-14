
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

[Authorize(Roles = "Admin")]
public class BooksController : Controller
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: BOOKS
    public async Task<IActionResult> Index()
    {
        var books = await _context.Books
            .Include(b => b.LibraryBranch)
            .ToListAsync();

        return View(books);
    }

    // GET: BOOKS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // GET: BOOKS/Create
    public IActionResult Create()
    {
        ViewData["LibraryBranchId"] = new SelectList(
            _context.LibraryBranches.OrderBy(b => b.Name),
            "Id",
            "Name");

        return View();
    }

    // POST: BOOKS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
    [Bind("Author,Genre,Id,Name,Description,LibraryCode,Status,LibraryBranchID")] Book book)
    {
        bool libraryCodeExists = await _context.Items
            .AnyAsync(i => i.LibraryCode == book.LibraryCode);

        if (libraryCodeExists)
        {
            ModelState.AddModelError(
                "LibraryCode",
                "This library code is already in use.");
        }

        if (ModelState.IsValid)
        {
            _context.Add(book);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        ViewData["LibraryBranchId"] = new SelectList(
    _context.LibraryBranches.OrderBy(b => b.Name),
    "Id",
    "Name",
    book.LibraryBranchId);

        return View(book);
    }

    // GET: BOOKS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        ViewData["LibraryBranchId"] = new SelectList(
    _context.LibraryBranches.OrderBy(b => b.Name),
    "Id",
    "Name",
    book.LibraryBranchId);
        return View(book);
    }

    // POST: BOOKS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Author,Genre,Id,Name,Description,LibraryCode,Status,LibraryBranchId")] Book book)
    {
        if (id != book.Id)
        {
            return NotFound();
        }
        bool libraryCodeExists = await _context.Items
    .AnyAsync(i => i.LibraryCode == book.LibraryCode && i.Id != book.Id);

        if (libraryCodeExists)
        {
            ModelState.AddModelError(
                "LibraryCode",
                "This library code is already in use.");
        }
        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(book.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        ViewData["LibraryBranchId"] = new SelectList(
    _context.LibraryBranches.OrderBy(b => b.Name),
    "Id",
    "Name",
    book.LibraryBranchId);
        return View(book);
    }

    // GET: BOOKS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var book = await _context.Books
            .FirstOrDefaultAsync(m => m.Id == id);
        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    // POST: BOOKS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var book = await _context.Books.FindAsync(id);
        if (book != null)
        {
            _context.Books.Remove(book);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool BookExists(int? id)
    {
        return _context.Books.Any(e => e.Id == id);
    }
}
