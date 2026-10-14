using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class LibraryBranch
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Branch Code")]
    public string BranchCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Address { get; set; } = string.Empty;

    public ICollection<Item> Items { get; set; } = new List<Item>();
}