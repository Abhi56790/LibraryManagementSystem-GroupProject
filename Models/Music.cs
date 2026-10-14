using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class Music : Item
{
    [Required]
    [StringLength(100)]
    public string Artist { get; set; } = string.Empty;

    [Range(1800, 2100)]
    public int Year { get; set; }
}