using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Reception")]

public class LoansController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFineCalculator _fineCalculator;

    public LoansController(
        ApplicationDbContext context,
        IFineCalculator fineCalculator)
    {
        _context = context;
        _fineCalculator = fineCalculator;
    }

    public async Task<IActionResult> History()
    {
        var returnedLoans = await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Borrower)
            .Where(l => l.ReturnDate != null)
            .OrderByDescending(l => l.ReturnDate)
            .ToListAsync();

        return View(returnedLoans);
    }

    // Shows the Borrow Item form
    [HttpGet]
    public async Task<IActionResult> Borrow()
    {
        var model = new BorrowItemViewModel();

        model.Borrowers = await _context.Borrowers
            .OrderBy(b => b.FullName)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.FullName
            })
            .ToListAsync();

        return View(model);
    }
    public async Task<IActionResult> Index()
    {
        var activeLoans = await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Borrower)
            .Where(l => l.ReturnDate == null)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

        return View(activeLoans);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Return(int id)
    {
        var loan = await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Borrower)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (loan == null)
        {
            return NotFound();
        }

        if (loan.ReturnDate != null)
        {
            TempData["SuccessMessage"] =
                "This item has already been returned.";

            return RedirectToAction(nameof(Index));
        }

        DateTime returnDate = DateTime.Today;

        decimal fine = _fineCalculator.CalculateFine(
            loan.DueDate,
            returnDate);

        loan.ReturnDate = returnDate;
        loan.FineAmount = fine;

        loan.Item.Status = ItemStatus.Available;

        await _context.SaveChangesAsync();

        if (fine > 0)
        {
            TempData["SuccessMessage"] =
                $"{loan.Item.Name} was returned successfully. Late fine: {fine:C}.";
        }
        else
        {
            TempData["SuccessMessage"] =
                $"{loan.Item.Name} was returned successfully. No fine was charged.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Borrow(BorrowItemViewModel model)
    {
        // Load borrowers again if the form needs to be shown again.
        model.Borrowers = await _context.Borrowers
            .OrderBy(b => b.FullName)
            .Select(b => new SelectListItem
            {
                Value = b.Id.ToString(),
                Text = b.FullName
            })
            .ToListAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Find the item using its library code.
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.LibraryCode == model.LibraryCode);

        if (item == null)
        {
            ModelState.AddModelError(
                "LibraryCode",
                "No item was found with this library code.");

            return View(model);
        }

        // Only available items can be borrowed.
        if (item.Status != ItemStatus.Available)
        {
            ModelState.AddModelError(
                "LibraryCode",
                "This item is not currently available.");

            return View(model);
        }

        // Make sure the borrower exists.
        var borrower = await _context.Borrowers
            .FindAsync(model.BorrowerId);

        if (borrower == null)
        {
            ModelState.AddModelError(
                "BorrowerId",
                "The selected borrower could not be found.");

            return View(model);
        }

        // Create the loan.
        var loan = new Loan
        {
            ItemId = item.Id,
            BorrowerId = borrower.Id,
            BorrowDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(14),
            FineAmount = 0
        };

        // The item is no longer available.
        item.Status = ItemStatus.Borrowed;

        _context.Loans.Add(loan);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"{item.Name} has been borrowed by {borrower.FullName}.";

        return RedirectToAction(nameof(Borrow));
    }
}