using Dipterv.Shared.Dto.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.ProductInventory
{
    public class ProductInventoryDto
    {
        public int ProductId { get; set; }
        public short LocationId { get; set; }
        public string Shelf { get; set; }
        public byte Bin { get; set; }
        public short Quantity { get; set; }
        public DateTime ModifiedDate { get; set; }
        public LocationDto Location { get; set; }
    }
}
