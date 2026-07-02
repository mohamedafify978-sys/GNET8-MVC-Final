using GYMsystem.DAL.Models;
using GYMSystem.BLL.Common;
using GYMSystem.BLL.ViewModels.MemberViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.Interface
{
    public interface IMemberServices
    {
        Task<IEnumerable<MemberViewModel>> GetMembersAsync(bool istracked,CancellationToken ct = default);

        Task<Result> CreateMemberAsync(CreateMemberViewModel model,CancellationToken ct=default);

        Task<MemberViewModel?> GetMemberDetailsByIdAsync(int id, CancellationToken ct = default);

        Task<HealthRecordViewModel?> GetMemberHealthRecordByIdAsync(int id, CancellationToken ct = default);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateByIdAsync(int id, CancellationToken ct = default);

        Task<Result> UpdateMemberAsync(int id,MemberToUpdateViewModel model, CancellationToken ct = default);
        Task<Result> RemoveMemberAsync(int id, CancellationToken ct = default);

    }
}
