using System.Text.Json.Serialization;

namespace Templates.TodoApp.Host.HostedService
{

    public class ProductInventoryChangeEvent
    {
        [JsonPropertyName("payload")]
        public ProductInventoryPayload Payload { get; set; }
    }

    public class ProductInventoryPayload
    {
        [JsonPropertyName("before")]
        public ProductInventoryData Before { get; set; }

        [JsonPropertyName("after")]
        public ProductInventoryData After { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; }

        [JsonPropertyName("op")]
        public string Op { get; set; }

        [JsonPropertyName("ts_ms")]
        public long TsMs { get; set; }

        [JsonPropertyName("transaction")]
        public object Transaction { get; set; }
    }

    public class ProductInventoryData
    {
        [JsonPropertyName("ProductID")]
        public int ProductID { get; set; }

        [JsonPropertyName("LocationID")]
        public int LocationID { get; set; }

        [JsonPropertyName("Shelf")]
        public string Shelf { get; set; }

        [JsonPropertyName("Bin")]
        public int Bin { get; set; }

        [JsonPropertyName("Quantity")]
        public int Quantity { get; set; }
    }
}
