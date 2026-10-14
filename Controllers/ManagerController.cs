using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Text;
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

    public async Task<IActionResult> Reports()
    {
        var today = DateTime.Today;

        var loans = await _context.Loans
            .Include(l => l.Item)
                .ThenInclude(i => i.LibraryBranch)
            .Include(l => l.Borrower)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

        ViewBag.TotalLoans = loans.Count;

        ViewBag.ActiveLoans = loans.Count(l =>
            l.ReturnDate == null);

        ViewBag.OverdueLoans = loans.Count(l =>
            l.ReturnDate == null &&
            l.DueDate.Date < today);

        ViewBag.TotalFineRevenue = loans.Sum(l =>
            l.FineAmount);

        ViewBag.TotalItems = await _context.Items.CountAsync();

        ViewBag.AvailableItems = await _context.Items
            .CountAsync(i => i.Status == ItemStatus.Available);

        ViewBag.BorrowedItems = await _context.Items
            .CountAsync(i => i.Status == ItemStatus.Borrowed);

        ViewBag.DamagedItems = await _context.Items
            .CountAsync(i => i.Status == ItemStatus.Damaged);

        ViewBag.DestroyedItems = await _context.Items
            .CountAsync(i => i.Status == ItemStatus.Destroyed);

        ViewBag.Branches = await _context.LibraryBranches
            .Select(b => new
            {
                b.Name,
                b.BranchCode,
                TotalItems = b.Items.Count(),
                AvailableItems = b.Items.Count(i =>
                    i.Status == ItemStatus.Available),
                BorrowedItems = b.Items.Count(i =>
                    i.Status == ItemStatus.Borrowed)
            })
            .OrderBy(b => b.Name)
            .ToListAsync();

        return View(loans);
    }

    public async Task<IActionResult> ExportLoansCsv()
    {
        var loans = await _context.Loans
            .Include(l => l.Item)
                .ThenInclude(i => i.LibraryBranch)
            .Include(l => l.Borrower)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

        var csv = new StringBuilder();

        csv.AppendLine(
            "Item,Library Code,Borrower,Branch,Borrow Date,Due Date,Return Date,Fine");

        foreach (var loan in loans)
        {
            string itemName = EscapeCsv(loan.Item.Name);
            string libraryCode = EscapeCsv(loan.Item.LibraryCode);
            string borrower = EscapeCsv(loan.Borrower.FullName);

            string branch = EscapeCsv(
                loan.Item.LibraryBranch?.Name ?? "Not Assigned");

            string returnDate = loan.ReturnDate.HasValue
                ? loan.ReturnDate.Value.ToString("dd/MM/yyyy")
                : "Active";

            csv.AppendLine(
                $"{itemName}," +
                $"{libraryCode}," +
                $"{borrower}," +
                $"{branch}," +
                $"{loan.BorrowDate:dd/MM/yyyy}," +
                $"{loan.DueDate:dd/MM/yyyy}," +
                $"{returnDate}," +
                $"{loan.FineAmount:F2}");
        }

        var bytes = Encoding.UTF8.GetBytes(csv.ToString());

        return File(
            bytes,
            "text/csv",
            $"library-loan-report-{DateTime.Today:yyyy-MM-dd}.csv");
    }

    private static string EscapeCsv(string value)
    {
        if (value.Contains(',') ||
            value.Contains('"') ||
            value.Contains('\n'))
        {
            value = "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }

    public async Task<IActionResult> ExportReportPdf()
    {
        var loans = await _context.Loans
            .Include(l => l.Item)
                .ThenInclude(i => i.LibraryBranch)
            .Include(l => l.Borrower)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

        ViewBag.GeneratedDate = DateTime.Now;
        ViewBag.TotalLoans = loans.Count;
        ViewBag.ActiveLoans = loans.Count(l => l.ReturnDate == null);
        ViewBag.TotalFines = loans.Sum(l => l.FineAmount);

        return View(loans);
    }
}