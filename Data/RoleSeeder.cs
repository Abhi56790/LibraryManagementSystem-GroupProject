using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Data;

public static class RoleSeeder
{
    public static async Task SeedRolesAndUsersAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        string[] roles =
        {
            "Admin",
            "Reception",
            "Manager"
        };

        // Create roles if they do not already exist.
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Demo accounts used for the university assignment.
        await CreateUserAsync(
            userManager,
            "admin@library.com",
            "Admin123!",
            "Admin");

        await CreateUserAsync(
            userManager,
            "reception@library.com",
            "Reception123!",
            "Reception");

        await CreateUserAsync(
            userManager,
            "manager@library.com",
            "Manager123!",
            "Manager");
    }

    private static async Task CreateUserAsync(
        UserManager<IdentityUser> userManager,
        string email,
        string password,
        string role)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                throw new Exception(
                    $"Could not create user {email}: " +
                    string.Join(", ",
                        result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}