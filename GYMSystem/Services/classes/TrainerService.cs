using GYMsystem.DAL.Models;
using GYMsystem.DAL.Repositories.classes;
using GYMsystem.DAL.Repositories.interfaces;
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

        public TrainerService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var exists = await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Email == model.Email, ct); 
            if (exists)
                return false;
            if (await unitOfWork.GetRepository<Trainer>().AnyAsync(t => t.Phone == model.Phone, ct))
                return false;
            var Trainer = new Trainer()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                DateOfBirth = model.DateOfBirth,
                Specialty = model.specialization,
                Gender = model.gender,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    Street = model.Street,
                    City = model.City
                }
            };

            unitOfWork.GetRepository<Trainer>().Add(Trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);

            return result > 0;

        }



        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {

            var trainers = await unitOfWork.GetRepository<Trainer>().GetAllAsync(ct: ct);
            return trainers.Select(t => new TrainerViewModel()
            {
                Id = t.Id,
                Name = t.Name,
                Email = t.Email,
                Phone = t.Phone,
                specialization = t.Specialty.ToString(),


            });
        }
        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int id, CancellationToken ct = default)
        {
          var trainer = await  unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;
            else
                return new TrainerViewModel()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    specialization = trainer.Specialty.ToString(),
                    DateOfBirth = trainer.DateOfBirth.ToString(),
                    address = $"{trainer.Address.BuildingNumber}- {trainer.Address.Street}- {trainer.Address.City}",
                };
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int id, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return null;
            else
                return new TrainerToUpdateViewModel()
                {
                    Name = trainer.Name,
                    Email = trainer.Email,
                    Phone = trainer.Phone,
                    specialization = trainer.Specialty,
                    BuildingNumber = trainer.Address.BuildingNumber,
                    Street = trainer.Address.Street,
                    City = trainer.Address.City
                };


        }

        public async Task<bool> RemoveTrainerAsync(int id, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return false;
            var hasfuturesession = await unitOfWork.GetRepository<Session>().AnyAsync(s => s.TrainerId == id && s.StartDate > DateTime.Now, ct);
            if (hasfuturesession) return false;

            unitOfWork.GetRepository<Trainer>().Delete(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);

            return result >0;

        }

        public async Task<bool> UpdateTrainerDetailsAsync(int id, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var trainer = await unitOfWork.GetRepository<Trainer>().GetByIdAsync(id, ct);
            if (trainer is null) return false;
            var emailExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Email == model.Email && m.Id != id, ct);
            var phoneExists = await unitOfWork.GetRepository<Trainer>().AnyAsync(m => m.Phone == model.Phone && m.Id != id, ct);

            if (emailExists || phoneExists) return false;

            trainer.Email = model.Email;
            trainer.Phone = model.Phone;
            trainer.Address.City=model.City;
            trainer.Address.Street=model.Street;
            trainer.Specialty = model.specialization;
            trainer.UpdatedAt=DateTime.Now;
             
            unitOfWork.GetRepository<Trainer>().Update(trainer);
            var result = await unitOfWork.SaveChangesAsync(ct);
            return result > 0;



        }
    }
}
