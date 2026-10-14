using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Toy : Item
{
    [Required]
    [StringLength(50)]
    public string Type { get; set; } = string.Empty;

    [Range(0, 18)]
    [Display(Name = "Minimum Age")]
    public int MinimumAge { get; set; }
}