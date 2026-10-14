using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class BorrowItemViewModel
{
    [Required]
    [Display(Name = "Library Code")]
    public string LibraryCode { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Please select a borrower.")]
    [Display(Name = "Borrower")]
    public int BorrowerId { get; set; }

    public List<SelectListItem> Borrowers { get; set; } = new();
}