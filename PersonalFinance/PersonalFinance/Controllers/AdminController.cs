using BusinessLayer.Interfaces;
using BusinessLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PersonalFinance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PersonalFinance.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IUserService _userService;

        public AdminController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _userService.GetUserDTOList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUser(id);
            return View(user);
        }

        [HttpGet]
        [Route("admin/edituser/{userId:int}")]
        public async Task<IActionResult> EditUser([FromRoute]int userId)
        {
            var user = await _userService.GetUser(userId);
            var roles = await _userService.GetRoles();
            var model = new AdminEditUserModel() {
                UserId = user.UserId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = user.DateOfBirth,
                Roles = roles.Select(r => new RoleModel
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    IsChecked = user.UserRoles.Any(u => u.RoleId == r.RoleId)
                }).ToArray()
            };

            return PartialView("_EditPartial", model);

        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> EditUser(AdminEditUserModel model)
        {
            //validate user  
            if (!ModelState.IsValid)
                return PartialView("_EditPartial", model);

            var user = await _userService.GetUser(model.UserId);

            var editResult = await UpdateUserWithRoles(model, user);

            if(!editResult.IsSuccess)
            {
                ModelState.AddModelError("", editResult.Errors.First());
                return PartialView("_EditPartial", model);
            }

           return Ok("Success");
        }

        [HttpGet]
        [Route("admin/deleteuser/{userId:int}")]
        public async Task<ActionResult> GetForDelete(int userId)
        {
            var user = await _userService.GetUser(userId);
            return PartialView("_DeletePartial", user);
        }

        [HttpDelete]
        [Route("admin/deleteuser/{userId:int}")]
        public async Task<ActionResult> Delete(int userId)
        {
            await _userService.DeleteUser(userId);

            return Ok("Success");
        }

        private async Task<ServiceResult> UpdateUserWithRoles(AdminEditUserModel model, UserDTO user)
        {
            var userdto = new UserDTO()
            {
                UserId = model.UserId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                UserRoles = model.Roles.Where(r => r.IsChecked).Select(r => new DataLayer.Entities.Role
                {
                    RoleId = r.RoleId,
                    Name = r.Name
                }),
                Login = user.Login,
                Password = user.Password
            };

            var editResult = await _userService.UpdateUserProfile(userdto);

            if (!editResult.IsSuccess)
                return editResult;

            return await _userService.UpdateUserRole(userdto);
        }



    }
}
