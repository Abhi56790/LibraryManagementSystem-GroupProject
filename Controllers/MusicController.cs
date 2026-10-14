
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Data;
using Microsoft.AspNetCore.Authorization;

[Authorize(Roles = "Admin")]

public class MusicController : Controller
{
    private readonly ApplicationDbContext _context;

    public MusicController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: MUSICS
    public async Task<IActionResult> Index()    
    {
        return View(await _context.Music.ToListAsync());
    }

    // GET: MUSICS/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var music = await _context.Music
            .FirstOrDefaultAsync(m => m.Id == id);
        if (music == null)
        {
            return NotFound();
        }

        return View(music);
    }

    // GET: MUSICS/Create
    public IActionResult Create()
    {
        return View();
    }

    // POST: MUSICS/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Artist,Year,Id,Name,Description,LibraryCode,Status,Loans")] Music music)
    {
        bool libraryCodeExists = await _context.Items
                .AnyAsync(i => i.LibraryCode == music.LibraryCode);

        if (libraryCodeExists)
        {
            ModelState.AddModelError(
                "LibraryCode",
                "This library code is already in use.");
        }
        if (ModelState.IsValid)
        {
            _context.Add(music);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(music);
    }

    // GET: MUSICS/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var music = await _context.Music.FindAsync(id);
        if (music == null)
        {
            return NotFound();
        }
        return View(music);
    }

    // POST: MUSICS/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int? id, [Bind("Artist,Year,Id,Name,Description,LibraryCode,Status,Loans")] Music music)
    {
        if (id != music.Id)
        {
            return NotFound();
        }
        bool libraryCodeExists = await _context.Items
            .AnyAsync(i => i.LibraryCode == music.LibraryCode && i.Id != music.Id);

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
                _context.Update(music);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MusicExists(music.Id))
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
        return View(music);
    }

    // GET: MUSICS/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var music = await _context.Music
            .FirstOrDefaultAsync(m => m.Id == id);
        if (music == null)
        {
            return NotFound();
        }

        return View(music);
    }

    // POST: MUSICS/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int? id)
    {
        var music = await _context.Music.FindAsync(id);
        if (music != null)
        {
            _context.Music.Remove(music);
        }

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool MusicExists(int? id)
    {
        return _context.Music.Any(e => e.Id == id);
    }
}
