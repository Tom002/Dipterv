using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.Product
{
    public class FilterProductDto
    {
        public int? ProductSubcategoryId { get; set; }
        public int? ProductCategoryId { get; set; }
        public bool OnlyShowProductsInStock { get; set; } = true;
        public bool OnlyShowProductsAvalaible { get; set; } = true;
        public bool OnlyShowDiscountedProducts { get; set; } = false;
        public int MinRating { get; set; } = 0;
        public string? ProductName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
    }
}
