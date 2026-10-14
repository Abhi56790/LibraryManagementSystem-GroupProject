using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Reception")]
public class ReservationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReservationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var reservations = await _context.Reservations
            .Include(r => r.Item)
            .Include(r => r.Borrower)
            .OrderBy(r => r.ItemId)
            .ThenBy(r => r.ReservedDate)
            .ToListAsync();

        return View(reservations);
    }

    public IActionResult Create()
    {
        ViewData["ItemId"] = new SelectList(
            _context.Items
                .Where(i =>
    i.Status == ItemStatus.Borrowed ||
    i.Status == ItemStatus.Damaged)
                .OrderBy(i => i.Name),
            "Id",
            "Name");

        ViewData["BorrowerId"] = new SelectList(
            _context.Borrowers.OrderBy(b => b.FullName),
            "Id",
            "FullName");

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int itemId, int borrowerId)
    {
        var item = await _context.Items.FindAsync(itemId);
        var borrower = await _context.Borrowers.FindAsync(borrowerId);

        if (item == null || borrower == null)
        {
            TempData["ErrorMessage"] = "Invalid item or borrower.";
            return RedirectToAction(nameof(Create));
        }

        if (item.Status != ItemStatus.Borrowed &&
    item.Status != ItemStatus.Damaged)
        {
            TempData["ErrorMessage"] =
                "Reservations can only be made for borrowed or damaged items.";

            return RedirectToAction(nameof(Create));
        }

        bool alreadyWaiting = await _context.Reservations.AnyAsync(r =>
            r.ItemId == itemId &&
            r.BorrowerId == borrowerId &&
            r.Status == "Waiting");

        if (alreadyWaiting)
        {
            TempData["ErrorMessage"] =
                "This borrower is already waiting for this item.";

            return RedirectToAction(nameof(Create));
        }

        var reservation = new Reservation
        {
            ItemId = itemId,
            BorrowerId = borrowerId,
            ReservedDate = DateTime.Now,
            Status = "Waiting"
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Borrower added to the reservation queue.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var reservation = await _context.Reservations.FindAsync(id);

        if (reservation == null)
        {
            return NotFound();
        }

        if (reservation.Status == "Waiting")
        {
            reservation.Status = "Cancelled";
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}