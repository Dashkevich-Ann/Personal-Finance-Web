using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using DataLayer.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using PersonalFinance.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PersonalFinance.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IUserService _userService;

        public AccountController(IAuthService authService, IUserService userService)
        {
            _authService = authService;
            _userService = userService;
        }

        [HttpGet("login")]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost("login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginModel model, [FromQuery] string returnUrl)
        {
            var dbUser = await _authService.ValidateUser(model.Login, model.Password);

            if (ModelState.IsValid)
            {
                if (dbUser != null)
                {
                    await Authenticate(dbUser);

                    // Redirect
                    if (!string.IsNullOrEmpty(returnUrl))
                        return Redirect(returnUrl);
                    else
                        return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Login or(and) password are incorrect");
            }
            return View("login");
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        private IEnumerable<Claim> GetRoleClaims(UserDTO user)
        {
            return user.UserRoles.Select(x => new Claim(ClaimTypes.Role, x.Name));
        }

        private async Task Authenticate(UserDTO user)
        {
            // Create Claims 
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Login));
            claims.Add(new Claim(ClaimTypes.Name, user.Name));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.AddRange(GetRoleClaims(user));

            // Create Identity
            var claimsIdentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            // Create Principal 
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Sign In
            await HttpContext.SignInAsync(claimsPrincipal);
        }

        [HttpGet]
        public ViewResult Register()
        {
            var user = _authService.GetLoggedInUser(User);
            return View(new RegisterModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            var user = new UserDTO()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Login = model.Login,
                Password = model.Password,
                DateOfBirth = model.DateOfBirth
            };

            var createUser = await _userService.CreateUser(user);

            if (!createUser.IsSuccess)
            {
                ModelState.AddModelError("", createUser.Errors.First());
                return PartialView("Register", model);
            }

            await Authenticate(user);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Route("profile")]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _authService.GetLoggedInUser(User);

            var model = new EditProfileModel()
            {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Login = user.Login           
            };
            return View("EditProfile", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("profile")]
        public async Task<ActionResult> EditProfile(EditProfileModel model)
        {
            //validate user  
            if (!ModelState.IsValid)
                return View("EditProfile", model);

            var editResult = await UpdateUserProfile(model);

            if (!editResult.IsSuccess)
            {
                ModelState.AddModelError("", editResult.Errors.First());
                return View("EditProfile", model);
            }

            return Ok("Success");
        }

        private async Task<ServiceResult> UpdateUserProfile(EditProfileModel model)
        {
            var user = new UserDTO()
            {
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Login = model.Login,
                Password = model.Password
            };

            var editResult = await _userService.UpdateUserProfile(user);

            return editResult;

        }

    }
}

