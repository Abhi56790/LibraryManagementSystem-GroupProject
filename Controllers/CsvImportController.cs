using LibraryManagementSystem.Data;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Controllers;

[Authorize(Roles = "Admin")]
public class CsvImportController : Controller
{
    private readonly ApplicationDbContext _context;

    public CsvImportController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View(new CsvItemImportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(CsvItemImportViewModel model)
    {
        if (model.CsvFile == null || model.CsvFile.Length == 0)
        {
            TempData["ErrorMessage"] = "Please select a CSV file.";
            return RedirectToAction(nameof(Index));
        }

        if (!Path.GetExtension(model.CsvFile.FileName)
            .Equals(".csv", StringComparison.OrdinalIgnoreCase))
        {
            TempData["ErrorMessage"] = "Only CSV files are supported.";
            return RedirectToAction(nameof(Index));
        }

        int importedCount = 0;
        int skippedCount = 0;
        var errors = new List<string>();

        using var reader = new StreamReader(model.CsvFile.OpenReadStream());

        // Skip header row.
        var header = await reader.ReadLineAsync();

        if (string.IsNullOrWhiteSpace(header))
        {
            TempData["ErrorMessage"] = "The CSV file is empty.";
            return RedirectToAction(nameof(Index));
        }

        int rowNumber = 1;

        while (!reader.EndOfStream)
        {
            rowNumber++;

            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var columns = line.Split(',');

            if (columns.Length < 11)
            {
                errors.Add($"Row {rowNumber}: Incorrect number of columns.");
                skippedCount++;
                continue;
            }

            string type = columns[0].Trim();
            string name = columns[1].Trim();
            string description = columns[2].Trim();
            string libraryCode = columns[3].Trim();
            string branchCode = columns[4].Trim();

            string author = columns[5].Trim();
            string genre = columns[6].Trim();

            string artist = columns[7].Trim();
            string yearText = columns[8].Trim();

            string toyType = columns[9].Trim();
            string minimumAgeText = columns[10].Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(description) ||
                string.IsNullOrWhiteSpace(libraryCode))
            {
                errors.Add(
                    $"Row {rowNumber}: Name, Description and LibraryCode are required.");

                skippedCount++;
                continue;
            }

            // Prevent duplicate library codes.
            bool duplicate = await _context.Items
                .AnyAsync(i => i.LibraryCode == libraryCode);

            if (duplicate)
            {
                errors.Add(
                    $"Row {rowNumber}: Library code '{libraryCode}' already exists.");

                skippedCount++;
                continue;
            }

            // Find branch from BranchCode.
            var branch = await _context.LibraryBranches
                .FirstOrDefaultAsync(b => b.BranchCode == branchCode);

            if (branch == null)
            {
                errors.Add(
                    $"Row {rowNumber}: Branch '{branchCode}' was not found.");

                skippedCount++;
                continue;
            }

            Item? item = null;

            if (type.Equals("Book", StringComparison.OrdinalIgnoreCase))
            {
                item = new Book
                {
                    Name = name,
                    Description = description,
                    LibraryCode = libraryCode,
                    LibraryBranchId = branch.Id,
                    Status = ItemStatus.Available,
                    Author = author,
                    Genre = genre
                };
            }
            else if (type.Equals("Music", StringComparison.OrdinalIgnoreCase))
            {
                int.TryParse(yearText, out int year);

                item = new Music
                {
                    Name = name,
                    Description = description,
                    LibraryCode = libraryCode,
                    LibraryBranchId = branch.Id,
                    Status = ItemStatus.Available,
                    Artist = artist,
                    Year = year
                };
            }
            else if (type.Equals("Toy", StringComparison.OrdinalIgnoreCase))
            {
                int.TryParse(minimumAgeText, out int minimumAge);

                item = new Toy
                {
                    Name = name,
                    Description = description,
                    LibraryCode = libraryCode,
                    LibraryBranchId = branch.Id,
                    Status = ItemStatus.Available,
                    Type = toyType,
                    MinimumAge = minimumAge
                };
            }
            else
            {
                errors.Add(
                    $"Row {rowNumber}: Unsupported item type '{type}'.");

                skippedCount++;
                continue;
            }

            _context.Items.Add(item);
            await _context.SaveChangesAsync();

            importedCount++;
        }

        TempData["SuccessMessage"] =
            $"{importedCount} item(s) imported. {skippedCount} row(s) skipped.";

        if (errors.Any())
        {
            TempData["ErrorMessage"] =
                string.Join(" | ", errors.Take(5));

            if (errors.Count > 5)
            {
                TempData["ErrorMessage"] +=
                    $" | Plus {errors.Count - 5} additional error(s).";
            }
        }

        return RedirectToAction(nameof(Index));
    }
}