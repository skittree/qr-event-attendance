using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using register_app.Services;
using register_app.ViewModels;
using System;
using System.Threading.Tasks;

namespace register_app.Controllers
{
    public class EventController : Controller
    {
        private IEventService EventService { get; }
        public EventController(IEventService eventService)
        {
            EventService = eventService;
        }
        // GET: EventController
        public async Task<ActionResult> Index()
        {
            var indexViewModel = await EventService.GetIndexViewModelAsync();
            return View(indexViewModel);
        }

        // GET: EventController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            try
            {
                var detailsViewModel = await EventService.GetViewModelAsync(id);
                return View(detailsViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // GET: EventController/Create
        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            var model = EventService.GetCreateViewModel();
            return View(model);
        }

        // POST: EventController/Create
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var createViewModel = EventService.GetCreateViewModel();
                return View(createViewModel);
            }
            try
            {
                await EventService.CreateAsync(model, User);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // GET: EventController/Edit/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var editViewModel = await EventService.GetEditViewModelAsync(id, User);
                return View(editViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // POST: EventController/Edit/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EventEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await EventService.EditAsync(model, User);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }


        // GET: EventController/Delete/5
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleteViewModel = await EventService.GetDeleteViewModelAsync(id, User);
                return View(deleteViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        // POST: EventController/Delete/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(EventDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await EventService.DeleteAsync(model, User);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }
    }
}
