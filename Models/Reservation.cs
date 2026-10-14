using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Reservation
{
    public int Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    public Item Item { get; set; } = null!;

    [Required]
    public int BorrowerId { get; set; }

    public Borrower Borrower { get; set; } = null!;

    [Display(Name = "Reserved Date")]
    public DateTime ReservedDate { get; set; } = DateTime.Now;

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Waiting";
}