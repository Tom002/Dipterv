using System.Text.Json.Serialization;

namespace Templates.TodoApp.Host.HostedService
{
    public class ProductChangeEvent
    {
        [JsonPropertyName("payload")]
        public Payload Payload { get; set; }
    }

    public class Payload
    {
        [JsonPropertyName("before")]
        public ProductData Before { get; set; }

        [JsonPropertyName("after")]
        public ProductData After { get; set; }

        [JsonPropertyName("source")]
        public Source Source { get; set; }

        [JsonPropertyName("op")]
        public string Op { get; set; }

        [JsonPropertyName("ts_ms")]
        public long TsMs { get; set; }

        [JsonPropertyName("transaction")]
        public object Transaction { get; set; }
    }

    public class Source
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("connector")]
        public string Connector { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ts_ms")]
        public long TsMs { get; set; }

        [JsonPropertyName("snapshot")]
        public string Snapshot { get; set; }

        [JsonPropertyName("db")]
        public string Db { get; set; }

        [JsonPropertyName("sequence")]
        public object Sequence { get; set; }

        [JsonPropertyName("schema")]
        public string Schema { get; set; }

        [JsonPropertyName("table")]
        public string Table { get; set; }

        [JsonPropertyName("change_lsn")]
        public string ChangeLsn { get; set; }

        [JsonPropertyName("commit_lsn")]
        public string CommitLsn { get; set; }

        [JsonPropertyName("event_serial_no")]
        public int? EventSerialNo { get; set; }
    }

    public class ProductData
    {
        [JsonPropertyName("ProductID")]
        public int ProductID { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("ProductNumber")]
        public string ProductNumber { get; set; }

        [JsonPropertyName("MakeFlag")]
        public bool MakeFlag { get; set; }

        [JsonPropertyName("FinishedGoodsFlag")]
        public bool FinishedGoodsFlag { get; set; }

        [JsonPropertyName("Color")]
        public object Color { get; set; }

        [JsonPropertyName("SafetyStockLevel")]
        public int SafetyStockLevel { get; set; }

        [JsonPropertyName("ReorderPoint")]
        public int ReorderPoint { get; set; }

        [JsonPropertyName("StandardCost")]
        public string StandardCost { get; set; }

        [JsonPropertyName("ListPrice")]
        public string ListPrice { get; set; }

        [JsonPropertyName("Size")]
        public string Size { get; set; }

        [JsonPropertyName("SizeUnitMeasureCode")]
        public object SizeUnitMeasureCode { get; set; }

        [JsonPropertyName("WeightUnitMeasureCode")]
        public object WeightUnitMeasureCode { get; set; }

        [JsonPropertyName("Weight")]
        public object Weight { get; set; }

        [JsonPropertyName("DaysToManufacture")]
        public int DaysToManufacture { get; set; }

        [JsonPropertyName("ProductLine")]
        public object ProductLine { get; set; }

        [JsonPropertyName("Class")]
        public object Class { get; set; }

        [JsonPropertyName("Style")]
        public object Style { get; set; }

        [JsonPropertyName("ProductSubcategoryID")]
        public object ProductSubcategoryID { get; set; }

        [JsonPropertyName("ProductModelID")]
        public object ProductModelID { get; set; }

        [JsonPropertyName("SellStartDate")]
        public long SellStartDate { get; set; }

        [JsonPropertyName("SellEndDate")]
        public object SellEndDate { get; set; }

        [JsonPropertyName("DiscontinuedDate")]
        public object DiscontinuedDate { get; set; }

        [JsonPropertyName("rowguid")]
        public string Rowguid { get; set; }

        [JsonPropertyName("ModifiedDate")]
        public long ModifiedDate { get; set; }
    }
}
