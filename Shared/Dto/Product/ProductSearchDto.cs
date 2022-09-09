using Stl.Fusion.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.Product
{
    public class ProductSearchDto
    {
        public FilterProductDto Filter { get; set; } = new();

        public ProductPageRequest PageRequest { get; set; } = new();

        public Session Session { get; set; }
    }
}
