#region License

// // Copyright (c) Grindal-IT. All Right Reserved.
// // Licensed under the Apache License, Version 2.0.

#endregion

using BoutiqueQuantity.Data;
using Microsoft.AspNetCore.Identity;

namespace BoutiqueQuantity;

public class CreateAdminWithRole
{
    #region Methods

    public static async Task Create(IServiceProvider serviceProvider)
    {
        _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        bool roleExist = await _roleManager.RoleExistsAsync("Admin");
        var userExist = await _userManager.FindByEmailAsync("admin@email.com");

        if (!roleExist || userExist == null)
        {
            string roleName = "Admin";
            if (!roleExist)
            {
                var role = new IdentityRole { Name = roleName };
                await _roleManager.CreateAsync(role);
            }

            if (userExist == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin@email.com",
                    Email = "admin@email.com",
                    EmailConfirmed = true
                };
                const string userPassword = "Twoandahalfmen22!";
                IdentityResult checkUser = await _userManager.CreateAsync(user, userPassword);

                if (checkUser.Succeeded)
                    await _userManager.AddToRoleAsync(user, roleName);
            }
        }
    }

    #endregion

    #region Fields

    private static RoleManager<IdentityRole> _roleManager;
    private static UserManager<ApplicationUser> _userManager;

    #endregion
}
