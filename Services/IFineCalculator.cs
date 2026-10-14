namespace LibraryManagementSystem.Services;

public interface IFineCalculator
{
    decimal CalculateFine(DateTime dueDate, DateTime returnDate);
}