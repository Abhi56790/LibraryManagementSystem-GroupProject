using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public abstract class Item
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string LibraryCode { get; set; } = string.Empty;

    public ItemStatus Status { get; set; } = ItemStatus.Available;

    [Display(Name = "Library Branch")]
    public int? LibraryBranchId { get; set; }

    public LibraryBranch? LibraryBranch { get; set; }

    public ICollection<Loan> Loans { get; set; } = new List<Loan>();
}