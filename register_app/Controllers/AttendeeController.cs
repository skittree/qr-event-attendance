using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace register_app.Controllers
{
    public class AttendeeController : Controller
    {
        // GET: AttendeeController
        public ActionResult Index()
        {
            return View();
        }

        // GET: AttendeeController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: AttendeeController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AttendeeController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AttendeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: AttendeeController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AttendeeController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
