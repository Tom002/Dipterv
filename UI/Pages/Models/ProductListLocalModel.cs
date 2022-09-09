using Dipterv.Shared.Dto.Product;

namespace Dipterv.UI.Pages.Models
{
    public class ProductListLocalModel
    {
        public FilterProductDto Filter { get; set; } = new();

        public ProductPageRequest PageRequest { get; set; } = new();
    }
}
