namespace LibraryManagementSystem.Services;

public class FineCalculator : IFineCalculator
{
    private const decimal DailyFine = 1.00m;

    public decimal CalculateFine(DateTime dueDate, DateTime returnDate)
    {
        if (returnDate.Date <= dueDate.Date)
        {
            return 0;
        }

        int daysLate = (returnDate.Date - dueDate.Date).Days;

        return daysLate * DailyFine;
    }
}