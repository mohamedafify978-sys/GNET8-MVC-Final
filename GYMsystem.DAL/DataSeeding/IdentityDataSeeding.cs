using GYMsystem.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMsystem.DAL.DataSeeding
{
    public class IdentityDataSeeding
    {
        public static async Task seedIdentityDataAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = await userManager.Users.AnyAsync(ct);
                bool HasRoles = await roleManager.Roles.AnyAsync(ct);
                if (HasRoles && HasUsers) return;
                if (!HasRoles)
                {
                    var Roles = new List<IdentityRole>()
                    {
                        new IdentityRole(){Name = "SuperAdmin"},
                        new IdentityRole(){Name = "Admin"}
                    };

                    foreach (var roleName in Roles.Select(R => R.Name))
                    {
                        if (!await roleManager.RoleExistsAsync(roleName!))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName!));
                            if (!roleResult.Succeeded)
                                logger.LogError("Failed to create role {Role}: {Errors}", roleName,
                                    string.Join("; ", roleResult.Errors.Select(e => e.Description)));
                        }
                    }
                }
                if (!HasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Mohamed",
                        LastName = "Afify",
                        UserName = "MohamedAfify",
                        Email = "MohamedAfify@gmail.com",
                        PhoneNumber = "01152730331"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Amr",
                        LastName = "Mohamed",
                        UserName = "AmrMohamed",
                        Email = "AmrMohamed@gmail.com",
                        PhoneNumber = "01554455565"
                    };
                      var result= await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");
                    logger.LogInformation("Identity data seeding completed successfully.");
                }


            
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding identity data.");
                return;
                
            }
        }
    }
}
