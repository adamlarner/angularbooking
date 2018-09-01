using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AngularBooking.Data;
using AngularBooking.Models;
using AngularBooking.Models.View.Account;
using AngularBooking.Services.Email;
using AngularBooking.Services.JwtManager;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;


namespace AngularBooking.Controllers.Account
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [EnableCors("Default")]
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        private UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private SignInManager<User> _signInManager;
        private IJwtManager _jwtManager;
        private IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private IAntiforgery _antiforgery;

        public AccountController(IUnitOfWork unitOfWork, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, SignInManager<User> signInManager, IJwtManager jwtManager, IEmailService emailService, IConfiguration configuration, IAntiforgery antiforgery)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtManager = jwtManager;
            _emailService = emailService;
            _configuration = configuration;
            _antiforgery = antiforgery;
        }

        [AllowAnonymous]
        [HttpGet("getCSRFToken")]
        public IActionResult GetAntiForgeryToken()
        {
            var token = _antiforgery.GetAndStoreTokens(HttpContext);
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", token.RequestToken);

            return Ok(new { token = token.RequestToken, tokenName = token.HeaderName });
        }

        [AllowAnonymous]
        [HttpGet("isAuth")]
        public IActionResult IsAuth()
        {
            // check whether user is authenticated
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return Ok(true);
            }
            else
            {
                // clean up potential expired client-side auth cookie
                HttpContext.Response.Cookies.Delete("Authorization");
                return Ok(false);
            }
        }

        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = new User
            {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(error.Code, error.Description);
                return BadRequest(ModelState);
            }

            // create customer record and associate with user account
            Customer customer = new Customer
            {
                UserId = user.Id,
                ContactEmail = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            if (!_unitOfWork.Customers.Create(customer))
            {
                // issue creating customer, so delete new user and return error
                await _userManager.DeleteAsync(user);
                return BadRequest();
            }

            // if first user, configure role for user and admin
            if (_userManager.Users.Count() == 1)
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = "user" });
                await _roleManager.CreateAsync(new IdentityRole { Name = "admin" });
                // add first user to admin role
                await _userManager.AddToRoleAsync(user, "admin");
            }

            // add to default user role
            await _userManager.AddToRoleAsync(user, "user");

            // send email confirmation request, is required
            bool isConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if(!isConfirmed)
            {
                string emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string callbackUrl = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, code = emailConfirmationToken },
                    HttpContext.Request.Scheme);
                string emailContent = $"Please click the following link to confirm registration: " + callbackUrl;

                _emailService.SendEmail(model.Email, "Confirm Account Registration", emailContent);
            }
            
            return Ok();

        }

        [AllowAnonymous]
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);

            if (result.Succeeded)
            {
                return Redirect("/email-confirmed");
            }

            return BadRequest();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            User user = await _userManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("login_error", "user not found");
                return BadRequest();
            }

            // return claims information
            List<Claim> claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (result.Succeeded)
            {
                var jwt = await _jwtManager.GenerateJwtStringAsync(model.Email, claims);
                if (jwt == null)
                {
                    ModelState.AddModelError("login_error", "authentication error");
                    return BadRequest(ModelState);
                }

                // return jwt in cookie
                HttpContext.Response.Cookies.Append("Authorization", "Bearer " + jwt, new CookieOptions { HttpOnly = true });

                string name = "user";

                return Ok(new
                {
                    role = roles.Contains("admin") ? "admin" : "user",
                    name = name
                });
            }
            else
            {
                if (result.IsLockedOut)
                    ModelState.AddModelError("login_error", "Account is locked out");
                else if (result.IsNotAllowed)
                    ModelState.AddModelError("login_error", "Account access is not allowed");
                else
                    ModelState.AddModelError("login_error", "Incorrect username/password");

                return BadRequest(ModelState);
            }

        }

        [ValidateAntiForgeryToken]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // check for auth token, and add to revocation list
            var tokenInfo = await HttpContext.AuthenticateAsync("Bearer");

            if (tokenInfo != null)
            {
                var token = new JwtSecurityToken(claims: tokenInfo.Ticket.Principal.Claims);
                _jwtManager.RevokeToken(token);
                // remove cookie
                HttpContext.Response.Cookies.Delete("Authorization");
            }

            return Ok();
        }

        [ValidateAntiForgeryToken]
        [HttpPost("refreshAuth")]
        public async Task<IActionResult> RefreshAuthToken()
        {
            // invalidate existing token
            var tokenInfo = await HttpContext.AuthenticateAsync("Bearer");

            if (tokenInfo == null)
                return BadRequest();

            var token = new JwtSecurityToken(claims: tokenInfo.Ticket.Principal.Claims);
            _jwtManager.RevokeToken(token);

            // create new auth token with sub claim from old token, and update cookie
            var sub = token.Claims.SingleOrDefault(f => f.Type == "sub");
            if (sub == null)
                return BadRequest();

            // return claims information
            User user = await _userManager.FindByNameAsync(sub.Value);
            if (user == null)
                return BadRequest();

            List<Claim> claims = new List<Claim>();
            var roles = await _userManager.GetRolesAsync(user);
            foreach (string role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var jwt = await _jwtManager.GenerateJwtStringAsync(sub.Value, claims);
            HttpContext.Response.Cookies.Append("Authorization", "Bearer " + jwt, new CookieOptions { HttpOnly = true });

            // todo: get user name from customer table
            string name = "user";

            return Ok(new
            {
                role = roles.Contains("admin") ? "admin" : "user",
                name = name
            });

        }

        // user management
        [ValidateAntiForgeryToken]
        [HttpPost("password_reset")]
        public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // only allow change if user is admin, or logged in user is changing own password
            User user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
                return BadRequest();

            if (_userManager.IsInRoleAsync(user, "admin").Result || user.UserName == model.Email)
            {
                // check password is valid, and change if so
                var signinResult = await _signInManager.CheckPasswordSignInAsync(user, model.CurrentPassword, false);

                if (signinResult.Succeeded)
                {
                    var changeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    if (changeResult.Succeeded)
                    {
                        return Ok();
                    }
                }
            }
            return BadRequest();
        }


    }
}