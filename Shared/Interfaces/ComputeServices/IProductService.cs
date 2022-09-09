using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Paging;
using Stl.CommandR.Configuration;
using Stl.Fusion;

namespace Dipterv.Shared.Interfaces
{
    public interface IProductService
    {
        [ComputeMethod]
        public Task<List<ProductCategoryDto>> GetCategories(CancellationToken cancellationToken);

        [ComputeMethod]
        public Task<ProductDto> TryGetProduct(int productId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductDto?>> TryGetManyProducts(List<int> productIdList, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<ProductPhotoDto?> TryGetProductPhoto(int productPhotoId, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductPhotoDto?>> TryGetManyProductPhotos(List<int> productPhotoIdList, CancellationToken cancellationToken = default);

        [ComputeMethod]
        public Task<List<ProductDto>> GetAll(string searchTerm = "", CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Edit(UpdateProductCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Add(AddProductCommand command, CancellationToken cancellationToken = default);

        [CommandHandler]
        public Task Delete(DeleteProductCommand command, CancellationToken cancellationToken = default);
    }
}
