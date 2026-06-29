using GYMSystem.BLL.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MVC1.Controllers
{
    public class SessionController : Controller
    {
        private readonly ISessionServices sessionServices;

        public SessionController(ISessionServices sessionServices)
        {
            this.sessionServices = sessionServices;
        }
        public async Task<IActionResult> Index(CancellationToken cancellationToken)
        {
            var sessions = await sessionServices.GetAllSessionsAsync(cancellationToken);

            return View(sessions);
        }
    }
}
