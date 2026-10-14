using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Manager")]
public class ManagerController : Controller
{
    private readonly ApplicationDbContext _context;

    public ManagerController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var today = DateTime.Today;

        var model = new ManagerDashboardViewModel
        {
            // Borrowing statistics
            TotalLoans = await _context.Loans.CountAsync(),

            ActiveLoans = await _context.Loans
                .CountAsync(l => l.ReturnDate == null),

            ReturnedLoans = await _context.Loans
                .CountAsync(l => l.ReturnDate != null),

            OverdueLoans = await _context.Loans
                .CountAsync(l =>
                    l.ReturnDate == null &&
                    l.DueDate < today),

            // Item status statistics
            TotalItems = await _context.Items.CountAsync(),

            AvailableItems = await _context.Items
                .CountAsync(i => i.Status == ItemStatus.Available),

            BorrowedItems = await _context.Items
                .CountAsync(i => i.Status == ItemStatus.Borrowed),

            DamagedItems = await _context.Items
                .CountAsync(i => i.Status == ItemStatus.Damaged),

            DestroyedItems = await _context.Items
                .CountAsync(i => i.Status == ItemStatus.Destroyed),

            // Fine statistics
            TotalFines = await _context.Loans
                .SumAsync(l => l.FineAmount),

            LoansWithFines = await _context.Loans
                .CountAsync(l => l.FineAmount > 0)
        };

        if (model.LoansWithFines > 0)
        {
            model.AverageFine = await _context.Loans
                .Where(l => l.FineAmount > 0)
                .AverageAsync(l => l.FineAmount);
        }

        return View(model);
    }
}