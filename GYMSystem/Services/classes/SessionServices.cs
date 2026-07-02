using AutoMapper;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Models.Enums;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Common;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.SessionViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class SessionServices : ISessionServices
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public SessionServices(IUnitOfWork unitOfWork,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct)
        {
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be after start date.");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start date must be in the future."); 
            if (model.Capacity <1 || model.Capacity >25) return Result.Validation("Capacity Must Be Between 1 And 25");

            var trainer =await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer == null) return Result.NotFound("Trainer not found.");

            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category not found.");

            var isValid = Enum.TryParse<Specialties>(category.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Cannot create this session for this trainer.");

            var session = mapper.Map<CreateSessionViewModel,Session>(model);

            unitOfWork.GetRepository<Session>().Add(session);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result >0 ? Result.Ok() : Result.Fail("Failed to create session.");
        }

        public async Task<Result> DeleteSessionAsync(int id, CancellationToken ct = default)
        {

            var session = await unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session == null) return Result.NotFound("Session not found.");
            if(session.EndDate >= DateTime.Now) return Result.Fail("Cannot delete a session that has not already ended Yet.");
            var bookingsCount = await unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(session.Id, ct);
            if (bookingsCount < 0)
                return Result.Fail("Cannot delete a session that has already been booked.");
            
            unitOfWork.GetRepository<Session>().Delete(session);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete session.");

        }

        public async Task<IEnumerable<SessionViewModel>?> GetAllSessionsAsync(CancellationToken ct)
        {
            var sessions = await unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(ct);
            if (sessions == null || !sessions.Any()) return null;

            var mapped = mapper.Map<IEnumerable<Session>, IEnumerable<SessionViewModel>>(sessions);

            foreach (var session in mapped) {
            
              session.AvailableSlots =  session.Capacity -await  unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(session.Id,ct);
            
            }
            return mapped;


        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoriesForDropDownAsync(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Category>().GetAllAsync(ct :ct);
            return mapper.Map<IEnumerable<CategorySelectViewModel>>(result);

         
        }

        public async Task<SessionViewModel> GetSessionByIdAsync(int id, CancellationToken ct = default)
        {
            var session =await unitOfWork.SessionRepository.GetSessionWithTrainerAndCategoryByIdAsync(id ,ct:ct );
            if (session == null) return null;
            else
            {
                var MappedSession = mapper.Map<Session, SessionViewModel>(session);
                MappedSession.AvailableSlots = MappedSession.Capacity - await unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(MappedSession.Id, ct);
                return MappedSession;
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdateAsync(int id, CancellationToken ct = default)
        {
            var session = await unitOfWork.GetRepository<Session>().GetByIdAsync(id, ct: ct);
            if (session == null) return Result<UpdateSessionViewModel>.NotFound("Session not found.");
            if(session.StartDate <= DateTime.Now) return Result<UpdateSessionViewModel>.Fail("Cannot update a session that has already started.");

            var bookingsCount = await unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(session.Id, ct);
            if (bookingsCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cannot update a session that has already been booked.");     

            var mapped = mapper.Map<Session, UpdateSessionViewModel>(session);

            
            return Result<UpdateSessionViewModel>.Ok(mapped);



        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainersForDropDownAsync(CancellationToken ct = default)
        {
            var result = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return mapper.Map<IEnumerable<TrainerSelectViewModel >> (result);
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await unitOfWork.GetRepository<Session>().GetByIdAsync(id, ct: ct);
            if (session == null) return Result.NotFound("Session not found.");
            if (session.StartDate <= DateTime.Now) return Result.Fail("Can not update a session that has already started.");

            var bookingsCount = await unitOfWork.SessionRepository.GetCountOfBookedSoltsAsync(session.Id, ct);
            if (bookingsCount > 0)
                return Result.Fail("Can not update a session that has already been booked.");
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be after start date.");
            if (model.StartDate <= DateTime.Now)
                return Result.Validation(
                    "Start date must be in the future.");



            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId, ct);
            if (trainer == null) return Result.NotFound("Trainer not found.");
            var category = await unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId, ct);
            var isValid = Enum.TryParse<Specialties>(category?.CategoryName, true, out var CategorySpecialty);
            if (!isValid || trainer.Specialty != CategorySpecialty) return Result.Validation("Cannot create this session for this trainer.");
         

             mapper.Map(model, session);
            session.UpdatedAt = DateTime.Now;
            unitOfWork.SessionRepository.Update(session);

            return await unitOfWork.SaveChangesAsync(ct) > 0 ? Result.Ok() : Result.Fail("Failed to update session.");

        }
    }
}
