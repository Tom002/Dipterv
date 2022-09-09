
namespace Dipterv.Shared.Dto.ProductPhoto
{
    public class ProductPhotoDto
    {
        public HashSet<int> ProductIdList { get; set; } = new();
        public int ProductPhotoId { get; set; }
        public byte[] ThumbNailPhoto { get; set; }
        public string ThumbnailPhotoFileName { get; set; }
        public byte[] LargePhoto { get; set; }
        public string LargePhotoFileName { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
