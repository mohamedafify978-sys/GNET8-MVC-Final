using AutoMapper;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Common;
using GYMSystem.BLL.Services.AttachmentService;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class MemberServices : IMemberServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public IAttachmentService AttachmentService { get; }

        public MemberServices(IUnitOfWork unitOfWork, IMapper mapper,IAttachmentService AttachmentService)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
             this.AttachmentService = AttachmentService;
        }

        public async Task<Result> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExists || phoneExists) return Result.NotFound("Email or Phone already exists");



            var member = mapper.Map<CreateMemberViewModel, Member>(model);

            var photoResult = await AttachmentService.UploadAsync(model.ProfileFile.OpenReadStream(), model.ProfileFile.FileName, "MembersPhoto", ct);
            if (!photoResult.Success || string.IsNullOrEmpty(photoResult.Value))
                return Result.Validation("Profile photo upload failed (check file type and size).");
            var photo = photoResult.Value;


            //var result  = await unitOfWork.GetRepository<Member>.AddAsync(member,ct);
            unitOfWork.GetRepository<Member>().Add(member);
            member.photo = photo;
            var result = await unitOfWork.SaveChangesAsync(ct);
            if (result > 0)
            {
                return Result.Ok();
            }
            else
            {
                 AttachmentService.Delete(member.photo, "MembersPhoto");

                  return Result.Fail("Failed to create member.");
            }
           
        }



        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync(bool istracked, CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAllAsync(false, ct: ct);

            if (!members.Any()) return [];

            var memberViewModels = mapper.Map<IEnumerable<Member>, IEnumerable< MemberViewModel> >(members);
            return memberViewModels;
        }
        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var Member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id);
            if (Member == null) return null;

            var member = mapper.Map<MemberViewModel>(Member);


            var activememberShip = await unitOfWork.GetRepository<Membership>().firstOrDefaultAsync(m => m.MemberId == id && m.EndDate > DateTime.Now);
           if(activememberShip is not null)
            {
                var plan = await unitOfWork.GetRepository<Plan>().GetByIdAsync(activememberShip.PlanId,ct);
                member.PlanName = plan?.Name;
                
                member.membershipStartDate = activememberShip.CreatedAt.ToShortDateString();
                member.membershipEndDate = activememberShip.EndDate.ToShortDateString();
            }
            return member;
        }

        public async Task<HealthRecordViewModel?> GetMemberHealthRecordByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<HealthRecord>().firstOrDefaultAsync(m => m.MemberId == id, ct:ct);
            if (member == null) return null;

            else return mapper.Map<HealthRecordViewModel>(member);
            
        }
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;
            return mapper.Map<Member,MemberToUpdateViewModel>(member);
        }

        public async Task<Result> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if(member == null) return Result.NotFound("Member not found");

            var emailExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
             var phoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (emailExists || phoneExists) return Result.NotFound("Email or Phone already exists");


            mapper.Map(model, member);  
            member.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Member>().Update(member);
            var result =await unitOfWork.SaveChangesAsync(ct);
            return result >0 ? Result.Ok() : Result.Fail("Failed to update Member.");
        }

        public async Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return Result.NotFound("Member not found");

            var hasfutureBooking = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct);
            if (hasfutureBooking) return Result.Fail("Cannot delete member with future bookings.", ResultKind.Conflict);
            unitOfWork.GetRepository<Member>().Delete(member);
            AttachmentService.Delete(member.photo, "MembersPhoto");
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");

        }

    }
}