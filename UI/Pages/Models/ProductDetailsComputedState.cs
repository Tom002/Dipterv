using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.ProductCategory;
using System.Collections.Generic;

namespace Templates.TodoApp.UI.Pages.Models
{
    public class ProductDetailsComputedState
    {
        public ProductDetailsDto ProductDetails { get; set; } = new();

        public List<ProductCategoryDto> ProductCategories { get; set; } = new();
    }
}
