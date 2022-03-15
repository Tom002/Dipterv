using Stl.CommandR;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto.ProductInventory
{
    public class AddProductInventoryCommand : ICommand<Unit>
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public short LocationId { get; set; }
        [Required]
        [StringLength(10)]
        public string Shelf { get; set; }
        public byte Bin { get; set; }
        public short Quantity { get; set; }
    }
}
