using Dipterv.Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Paging
{
    public class PageRequest
    {
        public int PageSize { get; set; } = 10;

        public int Page { get; set; } = 1;
    }
}
