using AutoMapper;
using GoWeb.Interfaces;
using GoWeb.Models;
using GoWebApplication.Db.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GoWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
   // [Authorize(Roles = "Администратор")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IUserRepository userManager;
        private readonly IMapper mapper;
        public RoleController(RoleManager<IdentityRole> roleManager, IUserRepository userManager, IMapper mapper) 
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.mapper = mapper;
        }


        public async Task<IActionResult> Create()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(RoleView role)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(role.Name))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role.Name));
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Роль успешно добавленна";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(role);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var role= await roleManager.FindByIdAsync(id);
            if (role != null)
            {
                TempData["SuccessMessage"] = "Роль успешно удалена";
                await roleManager.DeleteAsync(role);
            }
            else
            {
                TempData["ErrorMessage"] = "Такой роли не существует";
            }
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> EditRolesUser(string? userName)
        {
            if (userName != null)
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user != null)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    var allRole = await roleManager.Roles
                                                   .Where(r => !userRoles.Any(ur => r.Name == ur))
                                                   .Select(r => new SelectListItem { Text = r.Name, Value = r.Name })
                                                   .ToListAsync();
                    var userView = new UserRolesEditorViewModel
                    {
                        AllRoles = allRole,
                        DisplayName = user.DisplayName,
                        UserName = user.UserName,
                        RolesUser = userRoles
                    };
                    return View(userView);
                }
                else
                {
                    TempData["ErrorMessage"] = "Пользователь с таким никнеймом не найден";
                    return View();
                }
            }
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> EditRolesUser(UserRolesEditorViewModel userView)
        {
            if (ModelState.IsValid)
            {
                var userDB = await userManager.FindByNameAsync(userView.UserName);
                if (userDB != null)
                {
                    var result = await userManager.AddToRoleAsync(userDB, userView.SelectedRole);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = "Роль успешно добавлена пользователю";
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else 
                {
                    TempData["ErrorMessage"] = "Пользователь с таким никнеймом не найден";
                }

            }
            return RedirectToAction("EditRolesUser", new { userName = userView.UserName });

        }

        

        public async Task<ActionResult>  DeleteRoleUser(string nameUser, string nameRole)
        {
            var user=await userManager.FindByNameAsync(nameUser);
            if(user!=null)
            {
                var result= await userManager.RemoveFromRoleAsync(user, nameRole);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Роль успешно удалена у пользователя";
                    return RedirectToAction("EditRolesUser", new { userName = nameUser });
                }
                else 
                {
                    TempData["ErrorMessage"] = "Не удалось удалить роль у пользователя";
                }
            }
            TempData["ErrorMessage"] = "Пользователя с таким именем не существует";
            return RedirectToAction("EditRolesUser");

        }

        public async Task<ActionResult> EditRole(string id)
        {
            var role= await roleManager.FindByIdAsync(id);
            var roleView= mapper.Map <RoleView> (role);
            return View(roleView);
        }

        [HttpPost]
        public async Task<ActionResult> EditRole(RoleView roleView)
        {
            if (ModelState.IsValid)
            {
                var roleDb = await roleManager.FindByIdAsync(roleView.Id);
                mapper.Map(roleView, roleDb);
                var result = await roleManager.UpdateAsync(roleDb);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Роль успешно отредактирована";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            
            return View(roleView);
        }


        public async  Task<ActionResult> Index()
        {
            var roles= await roleManager.Roles.ToListAsync();
            var rolesViews= mapper.Map<List<RoleView>>(roles);
            return View(rolesViews);
        }

       
    }
}
