using Microsoft.AspNetCore.Identity;

namespace InSightWindowAPI.Enums
{
    public static class UserRoles
    {
        public const string ADMIN = "Admin";
        public const string USER = "User";
    }

    public static class RoleSeeder
    {

        private static readonly string[] Roles = new[] { UserRoles.ADMIN, UserRoles.USER };

        public static async Task SeedRolesAsync(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            foreach (var role in Roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }
        }
    }
}
