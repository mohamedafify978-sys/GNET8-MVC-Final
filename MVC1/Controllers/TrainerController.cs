using GYMSystem.BLL.Services.classes;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using GYMSystem.BLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MVC1.Controllers
{
    public class TrainerController : Controller
    {
        private readonly ITrainerService trainerService;

        public TrainerController(ITrainerService trainerService)
        {
            this.trainerService = trainerService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var trainer = await trainerService.GetAllTrainersAsync(ct);
            return View(trainer);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await trainerService.CreateTrainerAsync(model, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id ,CancellationToken ct)
        {
            var trainerDetails = await trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainerDetails is null)
            {
                TempData["ErrorMessage"] = "trainer not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainerDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var trainer = await trainerService.GetTrainerToUpdateAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);

        }

        [HttpPost]
        public async  Task<IActionResult>  Edit(int id, TrainerToUpdateViewModel Trainer, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(Trainer);


            var result = await trainerService.UpdateTrainerDetailsAsync(id, Trainer, ct);

            if (result.Success)
                TempData["SuccessMessage"] = "Member updated successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));

        }
        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var trainer = await trainerService.GetTrainerDetailsAsync(id, ct);
            if (trainer is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed( int id, CancellationToken ct)
        {
            //if (!ModelState.IsValid) return View(nameof(Delete), id);
            var result = await trainerService.RemoveTrainerAsync(id, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member deleted successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));

        }


    }


}

