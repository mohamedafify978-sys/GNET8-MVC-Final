using GYMsystem.DAL.Context;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.classes;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Threading.Tasks;

namespace MVC1.Controllers
{


    public class PlanController : Controller
    {
        private readonly IPlanServices planService;

        public PlanController(IPlanServices planService)
        {
            this.planService = planService;
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {

            return View(await planService.GetAllPlanAsync());
        }


        public async Task<IActionResult> Details(int id, CancellationToken token)
        {

            var planDetails = await planService.GetPlanByIdAsync(id, token);
            if (planDetails is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(planDetails);
        }

        [HttpGet]
         public async Task<IActionResult> Edit(int id, CancellationToken token)
        {
            var planToUpdate = await planService.GetPlanToUpdateAsync(id, token);
            if(planToUpdate is null)
            {
                TempData["ErrorMessage"] = "Plan not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(planToUpdate);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel model, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return View(model);
            var result = await planService.UpdatePlanAsync(id, model, token);
            if (result.Success)
                TempData["SuccessMessage"] = "Plan updated successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Activate(int id, CancellationToken token)
        {
            var result = await planService.ToggleActivatioAsync(id, token);
            if (result.Success)
                TempData["SuccessMessage"] = "Plan activation status toggled successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));
        }
    }
}