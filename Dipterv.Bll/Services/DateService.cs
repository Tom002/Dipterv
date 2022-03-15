using Dipterv.Shared.Interfaces.ComputeServices;
using Stl.Fusion;

namespace Dipterv.Bll.Services
{
    public class DateService : IDateService
    {
        [ComputeMethod]
        public virtual Task<DateTime> GetCurrentDate()
            => Task.FromResult(DateTime.Now.Date);
    }
}
