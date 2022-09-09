using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Paging;
using System.Collections.Generic;

namespace Templates.TodoApp.UI.Pages.Models
{
    public class ProductListComputedState
    {
        public List<ProductCategoryDto> ProductCategories { get; set; } = new();

        public PageResponse<ListProductDto> SearchResults { get; set; } = new(new List<ListProductDto>(), 1, 0);
    }
}
