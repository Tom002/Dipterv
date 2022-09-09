using System.Text.Json.Serialization;

namespace Dipterv.Shared.Dto.Product
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum ProductOrderByEnum
    {
        AverageReviews = 1,
        PriceHighToLow = 2,
        PriceLowToHigh = 3,
        NewestArrivals = 4
    }
}
