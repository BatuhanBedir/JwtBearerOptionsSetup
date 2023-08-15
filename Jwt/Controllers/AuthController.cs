using Jwt.Models;
using Jwt.Providers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Jwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenProvider _jwtTokenProvider;

        public AuthController(IJwtTokenProvider jwtTokenProvider)
        {
            _jwtTokenProvider = jwtTokenProvider;
        }
        [HttpPost("register")]  //From services-> inject etmek ile aynı şey. Bi controller içinde az sayıda kullanılacaksa. sadece ihtiyacım oldugu anda gelsin. Controllerla ayağa kalmıyor, injection ile aynı tek fark.
        public async Task<IActionResult> Register([FromServices] UserManager<AppUser> userManager, RegistrationDto registrationDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = new AppUser
            {
                Email = registrationDto.Email,
                EmailConfirmed = true,
                UserName = registrationDto.Email,
                FirstName = registrationDto.FirstName,
                LastName = registrationDto.LastName,
            };
            var identityCreateResult =await userManager.CreateAsync(user, registrationDto.Password);
            if (!identityCreateResult.Succeeded) {
                return BadRequest(new AuthResult
                {
                    IsSuccess = false,
                    Message = identityCreateResult.ToString()
                }); 
            }

            var token = _jwtTokenProvider.GenerateToken(user);
            return Ok(new AuthResult
            {
                IsSuccess=true,
                Token = token
            });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromServices] SignInManager<AppUser> signInManager, LoginDto loginDto)
        {
            if (!ModelState.IsValid) { return BadRequest(ModelState); }

            var user = await signInManager.UserManager.FindByEmailAsync(loginDto.Email);
            if (user is null) { return BadRequest(new AuthResult { IsSuccess = false, Message = "Invalid Request" }); }

            var signInResult = await signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!signInResult.Succeeded)
            {
                return BadRequest(new AuthResult { IsSuccess = false, Message = "Invalid Request" });
            }
            var token = _jwtTokenProvider?.GenerateToken(user);
            return Ok(new AuthResult
            {
                IsSuccess=true,
                Token = token
            });
        }
    }
}
