using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Reception")]
public class NotificationsController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var notifications = await _context.Notifications
            .Include(n => n.Borrower)
            .Include(n => n.Reservation)
                .ThenInclude(r => r!.Item)
            .Include(n => n.Loan)
                .ThenInclude(l => l!.Item)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

        return View(notifications);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckLoanNotifications()
    {
        var today = DateTime.Today;

        var activeLoans = await _context.Loans
            .Include(l => l.Item)
            .Include(l => l.Borrower)
            .Where(l => l.ReturnDate == null)
            .ToListAsync();

        int createdCount = 0;

        foreach (var loan in activeLoans)
        {
            string? type = null;
            string? message = null;

            // Overdue loan
            if (loan.DueDate.Date < today)
            {
                int overdueDays = (today - loan.DueDate.Date).Days;

                type = "Overdue";

                message =
                    $"'{loan.Item.Name}' is {overdueDays} day(s) overdue. " +
                    $"Please return the item as soon as possible.";
            }

            // Due within the next 3 days
            else if (loan.DueDate.Date <= today.AddDays(3))
            {
                type = "Nearly Due";

                message =
                    $"'{loan.Item.Name}' is due on " +
                    $"{loan.DueDate:dd/MM/yyyy}.";
            }

            if (type == null)
            {
                continue;
            }

            // Prevent duplicate notifications of the same type
            // for the same loan on the same day.
            bool alreadyCreated = await _context.Notifications.AnyAsync(n =>
                n.LoanId == loan.Id &&
                n.Type == type &&
                n.CreatedAt.Date == today);

            if (alreadyCreated)
            {
                continue;
            }

            var notification = new Notification
            {
                BorrowerId = loan.BorrowerId,
                LoanId = loan.Id,
                Type = type,
                Channel = "Email",
                Status = "Sent",
                CreatedAt = DateTime.Now,
                SentAt = DateTime.Now,
                Message = message
            };

            _context.Notifications.Add(notification);
            createdCount++;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] =
            $"{createdCount} loan notification(s) generated.";

        return RedirectToAction(nameof(Index));
    }
}