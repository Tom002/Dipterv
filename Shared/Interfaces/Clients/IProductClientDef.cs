using Dipterv.Shared.Dto;
using Dipterv.Shared.Dto.Product;
using Dipterv.Shared.Dto.ProductCategory;
using Dipterv.Shared.Dto.ProductPhoto;
using Dipterv.Shared.Paging;
using RestEase;

namespace Dipterv.Shared.Interfaces.Clients
{
    [BasePath("product")]
    public interface IProductClientDef
    {
        [Get("getCategories")]
        public Task<List<ProductCategoryDto>> GetCategories(CancellationToken cancellationToken);

        [Put("edit")]
        public Task Edit([Body] UpdateProductCommand command, CancellationToken cancellationToken = default);

        [Post("add")]
        public Task Add([Body] AddProductCommand command, CancellationToken cancellationToken = default);

        [Delete("delete")]
        public Task Delete([Body] DeleteProductCommand command, CancellationToken cancellationToken = default);

        [Get("tryGetProduct")]
        public Task<ProductDto> TryGetProduct(int productId, CancellationToken cancellationToken = default);

        [Get("tryGetManyProducts")]
        public Task<List<ProductDto?>> TryGetManyProducts(List<int> productIdList, CancellationToken cancellationToken = default);

        [Get("tryGetProductPhoto")]
        public Task<ProductPhotoDto?> TryGetProductPhoto(int productPhotoId, CancellationToken cancellationToken = default);

        [Get("tryGetManyProductPhotos")]
        public Task<List<ProductPhotoDto?>> TryGetManyProductPhotos(List<int> productPhotoIdList, CancellationToken cancellationToken = default);

        [Get("getAll")]
        public Task<List<ProductDto>> GetAll(string searchTerm = "", CancellationToken cancellationToken = default);
    }
}
