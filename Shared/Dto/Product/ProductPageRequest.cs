using Dipterv.Shared.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.Product
{
    public class ProductPageRequest : PageRequest
    {
        public ProductOrderByEnum Order { get; set; } = ProductOrderByEnum.AverageReviews;
    }
}
