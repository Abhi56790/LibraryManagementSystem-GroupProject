namespace LibraryManagementSystem.Models;

public class ManagerDashboardViewModel
{
    // Borrowing statistics
    public int TotalLoans { get; set; }
    public int ActiveLoans { get; set; }
    public int ReturnedLoans { get; set; }
    public int OverdueLoans { get; set; }

    // Item status statistics
    public int TotalItems { get; set; }
    public int AvailableItems { get; set; }
    public int BorrowedItems { get; set; }
    public int DamagedItems { get; set; }
    public int DestroyedItems { get; set; }

    // Fine statistics
    public decimal TotalFines { get; set; }
    public int LoansWithFines { get; set; }
    public decimal AverageFine { get; set; }
}