using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using register_app.Services;
using register_app.ViewModels;
using System;
using System.Threading.Tasks;

namespace register_app.Controllers
{
    public class AttendeeController : Controller
    {
        private IAttendeeService AttendeeService { get; }
        public AttendeeController(IAttendeeService attendeeService)
        {
            AttendeeService = attendeeService;
        }

        // GET: AttendeeController
        public async Task<ActionResult> Index(int id)
        {
            try
            {
                var indexViewModel = await AttendeeService.GetIndexViewModelAsync(id);
                return View(indexViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // GET: AttendeeController/Create
        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var model = await AttendeeService.GetCreateViewModelAsync(id);
            return View(model);
        }

        // POST: AttendeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendeeCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var createViewModel = AttendeeService.GetCreateViewModelAsync(model.EventId);
                return View(createViewModel);
            }
            try
            {
                await AttendeeService.CreateAsync(model, User);
                return RedirectToAction("Details", "Event", new { Id = model.EventId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // GET: AttendeeController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            try
            {
                var editViewModel = await AttendeeService.GetEditViewModelAsync(id, User);
                return View(editViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // POST: AttendeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AttendeeEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await AttendeeService.EditAsync(model, User);
                return RedirectToAction("Details", "Event", new { Id = model.EventId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // GET: AttendeeController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var deleteViewModel = await AttendeeService.GetDeleteViewModelAsync(id, User);
                return View(deleteViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // POST: AttendeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(AttendeeDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await AttendeeService.DeleteAsync(model, User);
                return RedirectToAction("Details", "Event", new { Id = model.EventId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }
    }
}
