using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Interfaces.ComputeServices
{
    public interface IDateService
    {
        public Task<DateTime> GetCurrentDate(); 
    }
}
