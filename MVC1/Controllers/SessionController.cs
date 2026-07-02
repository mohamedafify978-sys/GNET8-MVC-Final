using GYMSystem.BLL.Common;
using GYMSystem.BLL.Services.classes;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace MVC1.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionServices sessionServices;

        public SessionController(ISessionServices sessionServices)
        {
            this.sessionServices = sessionServices;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var sessions = await sessionServices.GetAllSessionsAsync(cancellationToken);

            return View(sessions);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CancellationToken ct) {
           
            await PopulateDropdownsAsync(ct);


            return View(); }
        

        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model,CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(nameof(Create), model);
            }

            var result = await sessionServices.CreateSessionAsync(model,ct);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session created successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropdownsAsync(ct);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var session = await sessionServices.GetSessionByIdAsync(id, ct);
            
              if (session is null)
                {
                    TempData["ErrorMessage"] = "Session not found.";
                    return RedirectToAction(nameof(Index));
                }
            return View(session);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await sessionServices.GetSessionToUpdateAsync(id, ct);
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Error;
                return RedirectToAction(nameof(Index));
            }
            await PopulateDropdownsAsync(ct);
            return View(result.Value);

        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(ct);
                return View(nameof(Edit), model);
            }
            var result = await sessionServices.UpdateSessionAsync(id, model, ct);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Session updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.Error;
            await PopulateDropdownsAsync(ct);
            return View(model);
        }
        [HttpGet]
       
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var session = await sessionServices.GetSessionByIdAsync(id, ct);
            if (session is null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
        {
            var result = await sessionServices.DeleteSessionAsync(id, ct);
            TempData[result.Success ? "SuccessMessage" : "ErrorMessage"] =
                result.Success ? "Session deleted successfully." : result.Error;
            return RedirectToAction(nameof(Index));
        }


        private async Task PopulateDropdownsAsync(CancellationToken ct)
        {
            ViewBag.trainers = new SelectList(await sessionServices.GetTrainersForDropDownAsync(), "Id", "Name");
            ViewBag.categories = new SelectList(await sessionServices.GetCategoriesForDropDownAsync(), "Id", "CategoryName");

        }
    }
}
