using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

[AllowAnonymous]
public class KioskController : Controller
{
    private readonly ApplicationDbContext _context;

    public KioskController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Account()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Account(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            TempData["AccountError"] =
                "Please enter your registered email address.";

            return RedirectToAction(nameof(Account));
        }

        var borrower = await _context.Borrowers
            .Include(b => b.Loans)
                .ThenInclude(l => l.Item)
            .FirstOrDefaultAsync(b => b.Email == email);

        if (borrower == null)
        {
            TempData["AccountError"] =
                "No library account was found with that email address.";

            return RedirectToAction(nameof(Account));
        }

        return View("AccountSummary", borrower);
    }

    public IActionResult Checkout()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(string email, string libraryCode)
    {
        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(libraryCode))
        {
            TempData["CheckoutError"] =
                "Please enter your email address and the item's library code.";

            return RedirectToAction(nameof(Checkout));
        }

        // Find the borrower.
        var borrower = await _context.Borrowers
            .FirstOrDefaultAsync(b => b.Email == email);

        if (borrower == null)
        {
            TempData["CheckoutError"] =
                "No library account was found with that email address.";

            return RedirectToAction(nameof(Checkout));
        }

        // Find the item.
        var item = await _context.Items
            .FirstOrDefaultAsync(i => i.LibraryCode == libraryCode);

        if (item == null)
        {
            TempData["CheckoutError"] =
                "No item was found with that library code.";

            return RedirectToAction(nameof(Checkout));
        }

        if (item.Status != ItemStatus.Available)
        {
            TempData["CheckoutError"] =
                $"'{item.Name}' is not currently available.";

            return RedirectToAction(nameof(Checkout));
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

        item.Status = ItemStatus.Borrowed;

        _context.Loans.Add(loan);

        // Save first so the loan receives its ID.
        await _context.SaveChangesAsync();

        // Simulated checkout notification.
        var notification = new Notification
        {
            BorrowerId = borrower.Id,
            LoanId = loan.Id,
            Type = "Item Borrowed",
            Channel = "Email",
            Status = "Sent",
            CreatedAt = DateTime.Now,
            SentAt = DateTime.Now,
            Message =
                $"'{item.Name}' was borrowed using the self-service kiosk. " +
                $"Due date: {loan.DueDate:dd/MM/yyyy}."
        };

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync();

        TempData["CheckoutSuccess"] =
            $"{item.Name} has been checked out successfully. " +
            $"Due date: {loan.DueDate:dd/MM/yyyy}.";

        return RedirectToAction(nameof(Checkout));
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Search(string libraryCode)
    {
        if (string.IsNullOrWhiteSpace(libraryCode))
        {
            TempData["ErrorMessage"] = "Please enter a library code.";
            return RedirectToAction(nameof(Index));
        }

        var item = await _context.Items
            .Include(i => i.LibraryBranch)
            .FirstOrDefaultAsync(i => i.LibraryCode == libraryCode);

        if (item == null)
        {
            TempData["ErrorMessage"] =
                "No library item was found with that code.";

            return RedirectToAction(nameof(Index));
        }

        return View("Item", item);
    }
}