using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Core.Entities.Identity;

namespace ECommerce.Core.Services_Contracts
{
    public interface IAuthService
    {
        Task<string> CreateTokenAsync(ApplicationUser user, UserManager<ApplicationUser> userManager);
    }
}

