using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.interfaces;
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

        public MemberServices(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            var emailExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email, ct);
            var phoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone, ct);
            if (emailExists || phoneExists) return false;

            var member = new Member
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street
                
                },
                HealthRecord = new HealthRecord
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Weight = model.HealthRecordViewModel.Weight,
                    Height = model.HealthRecordViewModel.Height,
                    Note = model.HealthRecordViewModel.Note
                }


            }; 
            //var result  = await unitOfWork.GetRepository<Member>.AddAsync(member,ct);
            unitOfWork.GetRepository<Member>().Add(member);
           var result =await  unitOfWork.SaveChangesAsync(ct);
            return result > 0;
        }



        public async Task<IEnumerable<MemberViewModel>> GetMembersAsync(bool istracked, CancellationToken ct = default)
        {
            var members = await unitOfWork.GetRepository<Member>().GetAllAsync(false, ct: ct);

            if (!members.Any()) return [];

            var memberViewModels = members.Select(m => new MemberViewModel
            {
                Id = m.Id,
                Name = m.Name,
                photo = m.photo,
                email = m.Email,
                phone = m.Phone,
                gender = m.Gender.ToString()

            });
            return memberViewModels;
        }
        public async Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default)
        {
            var Member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id);
            if (Member == null) return null;

            var member = new MemberViewModel()
            {
                Name = Member.Name,
                email = Member.Email,
                phone = Member.Phone,
                photo = Member.photo,
                DateofBirth = Member.DateOfBirth.ToShortDateString(),
                gender = Member.Gender.ToString(),
                Address = $"{Member.Address?.BuildingNumber}, {Member.Address?.Street}, {Member.Address?.City}",
            };


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

            else return new HealthRecordViewModel()
                {
                    Height = member.Height,
                    Weight = member.Weight,
                    BloodType = member.BloodType,
                    Note = member.Note
                };
            
        }
        public async Task<MemberToUpdateViewModel?> GetMemberToUpdateByIdAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return null;
            return new MemberToUpdateViewModel
            {
                Name = member.Name,
                Photo = member.photo,
                Email = member.Email,
                Phone = member.Phone,
                BuildingNumber = member.Address?.BuildingNumber ?? 0,
                City = member.Address?.City ?? string.Empty,
                Street = member.Address?.Street ?? string.Empty
            };
        }

        public async Task<bool> UpdateMemberAsync(int id, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if(member == null) return false;

            var emailExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
             var phoneExists = await unitOfWork.GetRepository<Member>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (emailExists || phoneExists) return false;
            
            
            member.Email = model.Email;
            member.Phone = model.Phone;
            member.Address.City = model.City;
            member.Address.Street = model.Street;
            member.UpdatedAt = DateTime.Now;

            unitOfWork.GetRepository<Member>().Update(member);
            var result =await unitOfWork.SaveChangesAsync(ct);
            return result >0 ;
        }

        public async Task<bool> RemoveMemberAsync(int id, CancellationToken ct = default)
        {
            var member = await unitOfWork.GetRepository<Member>().GetByIdAsync(id, ct);
            if (member == null) return false;

            var hasfutureBooking = await unitOfWork.GetRepository<Booking>().AnyAsync(b => b.MemberId == id && b.Session.StartDate > DateTime.Now, ct);
            if (hasfutureBooking) return false;
             unitOfWork.GetRepository<Member>().Delete(member);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0;

        }

    }
}