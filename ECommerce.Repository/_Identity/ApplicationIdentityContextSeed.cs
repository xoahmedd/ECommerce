using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities.Identity;

namespace ECommerce.Repository._Identity
{
    public class ApplicationIdentityContextSeed
    {
        public static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var user = new ApplicationUser()
                {
                    DisplayName = "Ahmed Mahmoud",
                    Email = "Ahmed@Mahmoud.com",
                    UserName = "xoahmed",
                    PhoneNumber = "0123456789"
                };

                await userManager.CreateAsync(user, "P@ssw0rd");
            }
        }
    }
}
