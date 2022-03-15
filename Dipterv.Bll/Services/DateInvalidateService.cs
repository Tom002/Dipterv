using Dipterv.Shared.Interfaces.ComputeServices;
using Stl.Fusion;

namespace Dipterv.Bll.Services
{
    public class DateInvalidateService
    {
        private readonly IDateService _dateService;

        public DateInvalidateService(IDateService dateService)
        {
            _dateService = dateService;
        }

        public async Task InvalidateCurrentDate()
        {
            using(Computed.Invalidate())
            {
                _ = await _dateService.GetCurrentDate();
            }
        }
    }
}
