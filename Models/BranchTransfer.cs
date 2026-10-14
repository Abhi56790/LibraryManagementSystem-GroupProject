using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class BranchTransfer
{
    public int Id { get; set; }

    [Required]
    public int ItemId { get; set; }

    public Item Item { get; set; } = null!;

    [Required]
    [Display(Name = "From Branch")]
    public int FromBranchId { get; set; }

    public LibraryBranch FromBranch { get; set; } = null!;

    [Required]
    [Display(Name = "To Branch")]
    public int ToBranchId { get; set; }

    public LibraryBranch ToBranch { get; set; } = null!;

    [Display(Name = "Requested Date")]
    public DateTime RequestedDate { get; set; } = DateTime.Now;

    [Display(Name = "Completed Date")]
    public DateTime? CompletedDate { get; set; }

    [Required]
    [StringLength(30)]
    public string Status { get; set; } = "Requested";
}