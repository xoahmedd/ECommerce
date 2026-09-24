using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ECommerce.APIs.DTOs;
using ECommerce.APIs.Errors;
using ECommerce.APIs.Extensions;
using ECommerce.Core.Entities.Identity;
using ECommerce.Core.Services_Contracts;

namespace ECommerce.APIs.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IAuthService authService, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _mapper = mapper;
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) return Unauthorized(new ApiResponse(401, "Invalid Login"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!result.Succeeded) return Unauthorized(new ApiResponse(401, "Invalid Login"));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            var user = new ApplicationUser()
            {
                DisplayName = model.DisplayName,
                Email = model.Email,
                UserName = model.Email.Split("@")[0],
                PhoneNumber = model.Phone
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });

            return Ok(new UserDto()
            {
                DisplayName =user.DisplayName,
                Email =user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet] // GET : /api/Account
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email)) return Unauthorized(new ApiResponse(401));

            var user = await _userManager.FindByEmailAsync(email);
            if (user is null) return Unauthorized(new ApiResponse(401));

            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _authService.CreateTokenAsync(user, _userManager)
            });
        }

        [Authorize]
        [HttpGet("address")] // GET : /api/Account/address
        public async Task<ActionResult<AddressDto>> GetUserAddress()
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user is null) return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<AddressDto>(user.Address));
        }

        [Authorize]
        [HttpPut("address")] // PUT : /api/Account/address
        public async Task<ActionResult<AddressDto>> UpdateUserAddress(AddressDto address)
        {
            var user = await _userManager.FindUserWithAddressAsync(User);
            if (user is null) return NotFound(new ApiResponse(404));

            var updatedAddress = _mapper.Map<Address>(address);

            if (user.Address is not null)
            {
                updatedAddress.Id = user.Address.Id;
            }

            user.Address = updatedAddress;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded) return BadRequest(new ApiValidationErrorResponse() { Errors = result.Errors.Select(E => E.Description) });

            return Ok(address);
        }
    }
}

