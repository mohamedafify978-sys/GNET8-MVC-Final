using AutoMapper;
using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.classes;
using GYMsystem.DAL.Repositories.interfaces;
using GYMSystem.BLL.Common;
using GYMSystem.BLL.Services.Interface;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using GYMSystem.BLL.ViewModels.TrainerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.classes
{
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public TrainerService(IUnitOfWork unitOfWork ,IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var exists = await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct); 
            if (exists)
                return Result.Fail("Email already exists", ResultKind.Conflict);
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct))
                return Result.Fail("Phone already exists", ResultKind.Conflict);
            var Trainer = mapper.Map<CreateTrainerViewModel, Trainer>(model);


            unitOfWork.GetRepository<Trainer>().Add(Trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");

        }



        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {

            var trainers = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return mapper.Map<IEnumerable<TrainerViewModel>>(trainers);


        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
          var trainer = await  unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;
            else
                return mapper.Map<TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;
            else
                return mapper.Map<TrainerToUpdateViewModel>(trainer);


        }

        public async Task<Result> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result.NotFound("Trainer not found");
            var hasfuturesession = await unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartDate > DateTime.Now, ct);
            if (hasfuturesession) return Result.Fail("Trainer has future sessions, cannot delete", ResultKind.Conflict);

            unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");

        }

        public async Task<Result> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return Result.NotFound("Trainer not found");
            var emailExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
            var phoneExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (emailExists || phoneExists) return Result.Fail("Email or Phone already exists", ResultKind.Conflict);

            mapper.Map(model, trainer);
            trainer.UpdatedAt=DateTime.Now;
             
            unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update Member.");



        }
    }
}
