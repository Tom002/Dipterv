using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto
{
    public class ProductWithReviewsDto : ProductDto
    {
        public List<ProductReviewDto> Reviews { get; set; } = new List<ProductReviewDto>();
    }
}
