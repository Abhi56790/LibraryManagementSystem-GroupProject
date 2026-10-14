using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class BranchTransfersController : Controller
{
    private readonly ApplicationDbContext _context;

    public BranchTransfersController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var transfers = await _context.BranchTransfers
            .Include(t => t.Item)
            .Include(t => t.FromBranch)
            .Include(t => t.ToBranch)
            .OrderByDescending(t => t.RequestedDate)
            .ToListAsync();

        return View(transfers);
    }

    public IActionResult Create()
    {
        ViewData["ItemId"] = new SelectList(
            _context.Items
                .Include(i => i.LibraryBranch)
                .Where(i => i.LibraryBranchId != null)
                .OrderBy(i => i.Name),
            "Id",
            "Name");

        ViewData["ToBranchId"] = new SelectList(
            _context.LibraryBranches.OrderBy(b => b.Name),
            "Id",
            "Name");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int itemId, int toBranchId)
    {
        var item = await _context.Items
            .Include(i => i.LibraryBranch)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null || item.LibraryBranchId == null)
        {
            TempData["ErrorMessage"] =
                "The selected item does not have a current branch.";

            return RedirectToAction(nameof(Create));
        }

        if (item.LibraryBranchId == toBranchId)
        {
            TempData["ErrorMessage"] =
                "The destination branch must be different from the current branch.";

            return RedirectToAction(nameof(Create));
        }

        var transfer = new BranchTransfer
        {
            ItemId = item.Id,
            FromBranchId = item.LibraryBranchId.Value,
            ToBranchId = toBranchId,
            RequestedDate = DateTime.Now,
            Status = "Requested"
        };

        _context.BranchTransfers.Add(transfer);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Branch transfer requested.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id)
    {
        var transfer = await _context.BranchTransfers
            .Include(t => t.Item)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transfer == null)
        {
            return NotFound();
        }

        if (transfer.Status != "Requested")
        {
            TempData["ErrorMessage"] =
                "This transfer has already been processed.";

            return RedirectToAction(nameof(Index));
        }

        transfer.Item.LibraryBranchId = transfer.ToBranchId;
        transfer.Status = "Completed";
        transfer.CompletedDate = DateTime.Now;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Transfer completed successfully.";

        return RedirectToAction(nameof(Index));
    }
}