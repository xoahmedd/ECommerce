using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using ECommerce.Core.Entities.Identity;

namespace ECommerce.APIs.Extensions
{
    public static class UserManagerExtension
    {
        public static async Task<ApplicationUser?> FindUserWithAddressAsync(this UserManager<ApplicationUser> userManager, ClaimsPrincipal User)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return null;

            var user = await userManager.Users.Include(U => U.Address).FirstOrDefaultAsync(U => U.NormalizedEmail == email.ToUpper());
           
            return user;
        }
    }
}

