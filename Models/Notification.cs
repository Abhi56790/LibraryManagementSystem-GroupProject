using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Notification
{
    public int Id { get; set; }

    [Required]
    public int BorrowerId { get; set; }

    public Borrower Borrower { get; set; } = null!;

    public int? LoanId { get; set; }

    public Loan? Loan { get; set; }

    public int? ReservationId { get; set; }

    public Reservation? Reservation { get; set; }

    [Required]
    [StringLength(30)]
    public string Type { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Message { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Channel { get; set; } = "Email";

    [Required]
    [StringLength(20)]
    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? SentAt { get; set; }
}