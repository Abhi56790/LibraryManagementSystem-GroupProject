using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Borrower
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    [StringLength(20)]
    public string? Phone { get; set; }

    [StringLength(200)]
    public string? Address { get; set; }

    [Display(Name = "Date Joined")]
    [DataType(DataType.Date)]
    public DateTime DateJoined { get; set; } = DateTime.Today;

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}