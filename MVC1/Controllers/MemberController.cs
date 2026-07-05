using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Services.AttachmentService;
using GYMSystem.BLL.Services.classes;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using System.Threading.Tasks;

namespace MVC1.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MemberController : Controller
    {
        private readonly IMemberServices memberServices;
        private readonly IAttachmentService attachmentService;

        public MemberController(IMemberServices memberServices,IAttachmentService attachmentService)
        {
            this.memberServices = memberServices;
            this.attachmentService = attachmentService;
        }
        [HttpGet]
        public async Task<IActionResult> picture(int id)
        {
            var member = await memberServices.GetMemberDetailsByIdAsync(id);
            if (member == null || string.IsNullOrWhiteSpace(member.photo))
                return NotFound();
            
         var result=    attachmentService.GetFile(member.photo, "MembersPhoto");
            if (result == null) return NotFound();


            return File(result.Value.Stream, result.Value.ContentType);
        }

        public async Task<IActionResult> Index(CancellationToken token)
        {
            var members = await memberServices.GetMembersAsync(false, token);
            return View(members);
        }
        [HttpGet]
        public IActionResult Create() => View();


        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken token)
        {
            if (!ModelState.IsValid)
                return View(nameof(Create), model);



            var result = await memberServices.CreateMemberAsync(model, token);
            if (result.Success)
                TempData["SuccessMessage"] = "Member created successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));

        }

        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var memberDetails = await memberServices.GetMemberDetailsByIdAsync(id, ct);
            if (memberDetails is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(memberDetails);
        }


        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var healthRecord = await memberServices.GetMemberHealthRecordByIdAsync(id, ct);
            if (healthRecord is null)
            {
                TempData["ErrorMessage"] = "Health record not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(healthRecord);
        }

        [HttpGet]
        public async Task<IActionResult> Editmember(int id, CancellationToken ct)
        {
            var memberToUpdate = await memberServices.GetMemberToUpdateByIdAsync(id, ct);
            if (memberToUpdate is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(memberToUpdate);

        }

        [HttpPost]
        public async Task<IActionResult> Editmember(int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);


            var result = await memberServices.UpdateMemberAsync(id, model, ct);

            if (result.Success)
                TempData["SuccessMessage"] = "Member updated successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index));

        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var memberToDelete = await memberServices.GetMemberDetailsByIdAsync(id, ct);
            if (memberToDelete is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(memberToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed([FromRoute]int id, CancellationToken ct)
        {
         //if (!ModelState.IsValid) return View(nameof(Delete), id);
            var result = await memberServices.RemoveMemberAsync(id, ct);
            if (result.Success)
                TempData["SuccessMessage"] = "Member deleted successfully.";
            else
                TempData["ErrorMessage"] = result.Error;
            return RedirectToAction(nameof(Index)); 

        } }   }

