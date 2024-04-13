using Microsoft.AspNetCore.Identity;

namespace MIL_LIT
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            string adminEmail = "admin@gmail.com";
            string password = "Qwerty_1";
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("admin"));
            }
            if (await roleManager.FindByNameAsync("user") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("user"));
            }
            if (await roleManager.FindByNameAsync("moderator") == null)
            {
                await roleManager.CreateAsync(new IdentityRole<int>("moderator"));
            }
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User { Email = adminEmail, UserName = adminEmail, PasswordHash = password, Login = adminEmail, IsAdmin = true};
                IdentityResult result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }

    }
}
