using GYMSystem.BLL.ViewModels.AnalytaicsViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GYMSystem.BLL.Services.Interface
{
    public interface IAnalyticsService
    {
        Task<AnalyticsViewModel> GetDataAsync(CancellationToken ct = default);
    }
}
