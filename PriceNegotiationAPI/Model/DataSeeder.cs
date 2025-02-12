using Microsoft.AspNetCore.Identity;

namespace PriceNegotiationAPI.Model
{
    public static class DataSeeder
    {
        public static async Task SeedUsers(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            if (!await roleManager.RoleExistsAsync("Employee"))
            {
                await roleManager.CreateAsync(new IdentityRole("Employee"));
            }

            var defaultUser = await userManager.FindByEmailAsync("default@user.com");
            var defaultEmployee = await userManager.FindByEmailAsync("default@employee.com");

            if (defaultUser == null)
            {
                defaultUser = new IdentityUser { UserName = "default@user.com", Email = "default@user.com" };
                await userManager.CreateAsync(defaultUser, "Password123!");
                await userManager.AddToRoleAsync(defaultUser, "User");
            }

            if (defaultEmployee == null)
            {
                defaultEmployee = new IdentityUser { UserName = "default@employee.com", Email = "default@employee.com" };
                await userManager.CreateAsync(defaultEmployee, "Password123!");
                await userManager.AddToRoleAsync(defaultEmployee, "Employee");
            }
        }
    }
}
