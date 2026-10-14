using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LibraryManagementSystem.Models;

public class CsvItemImportViewModel
{
    [Required]
    [Display(Name = "CSV File")]
    public IFormFile? CsvFile { get; set; }
}