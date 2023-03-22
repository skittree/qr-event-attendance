using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using register_app.Services;
using register_app.Areas.Identity.Pages.Account;
using register_app.ViewModels;

namespace Task3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IAdminService AdminService { get; }
        public AdminController(IAdminService adminService)
        {
            AdminService = adminService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await AdminService.GetIndexViewModelAsync();
            return View(model);
        }

        public IActionResult Register()
        {
            return View(AdminService.GetCreateModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await AdminService.RegisterAsync(model);
                return RedirectToAction("Index", "Admin");
            }
            catch (ArgumentException ae)
            {
                ModelState.AddModelError(nameof(model.UserName), ae.Message);
                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }


        }

        public async Task<IActionResult> Add(string id)
        {
            await AdminService.AddUserRoleAdmin(id);
            return RedirectToAction("Index");
        }
        

        public async Task<IActionResult> AddSecurity(string id)
        {
            await AdminService.AddUserRoleSecurity(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> AddOrganiser(string id)
        {
            await AdminService.AddUserRoleOrganiser(id);
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> Delete(string id)
        {
            await AdminService.RemoveRoleAdmin(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteOrganiser(string id)
        {
            await AdminService.RemoveRoleOrganiser(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteSecurity(string id)
        {
            await AdminService.RemoveRoleSecurity(id);
            return RedirectToAction("Index");
        }

       
    }
}
