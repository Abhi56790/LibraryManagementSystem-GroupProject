
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;
[Authorize(Roles = "Admin")]

public class LibraryBranchesController : Controller
{
    private readonly ApplicationDbContext _context;

    public LibraryBranchesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: LIBRARYBRANCHS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.LibraryBranches.ToListAsync());
    }

    // GET: LIBRARYBRANCHS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var librarybranch = await _context.LibraryBranches
            .FirstOrDefaultAsync(m => m.Id == id);
        if (librarybranch == null)
        {
            return NotFound();
        }

        return View(librarybranch);
    }

    // GET: LIBRARYBRANCHS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: LIBRARYBRANCHS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,BranchCode,Address,Items")] LibraryBranch librarybranch)
    {
        if (ModelState.IsValid)
        {
            _context.Add(librarybranch);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(librarybranch);
    }

    // GET: LIBRARYBRANCHS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var librarybranch = await _context.LibraryBranches.FindAsync(id);
        if (librarybranch == null)
        {
            return NotFound();
        }
        return View(librarybranch);
    }

    // POST: LIBRARYBRANCHS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Id,Name,BranchCode,Address,Items")] LibraryBranch librarybranch)
    {
        if (id != librarybranch.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(librarybranch);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LibraryBranchExists(librarybranch.Id))
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
        return View(librarybranch);
    }

    // GET: LIBRARYBRANCHS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var librarybranch = await _context.LibraryBranches
            .FirstOrDefaultAsync(m => m.Id == id);
        if (librarybranch == null)
        {
            return NotFound();
        }

        return View(librarybranch);
    }

    // POST: LIBRARYBRANCHS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var librarybranch = await _context.LibraryBranches.FindAsync(id);
        if (librarybranch != null)
        {
            _context.LibraryBranches.Remove(librarybranch);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool LibraryBranchExists(int? id)
    {
        return _context.LibraryBranches.Any(e => e.Id == id);
    }
}
