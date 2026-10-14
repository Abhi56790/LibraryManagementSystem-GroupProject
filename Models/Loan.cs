using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryManagementSystem.Models;

public class Loan
{
    public int Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    public Item Item { get; set; } = null!;

    [Required]
    public int BorrowerId { get; set; }

    public Borrower Borrower { get; set; } = null!;

    [Display(Name = "Borrow Date")]
    [DataType(DataType.Date)]
    public DateTime BorrowDate { get; set; }

    [Display(Name = "Due Date")]
    [DataType(DataType.Date)]
    public DateTime DueDate { get; set; }

    [Display(Name = "Return Date")]
    [DataType(DataType.Date)]
    public DateTime? ReturnDate { get; set; }

    [Display(Name = "Fine Amount")]
    [DataType(DataType.Currency)]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal FineAmount { get; set; }
}