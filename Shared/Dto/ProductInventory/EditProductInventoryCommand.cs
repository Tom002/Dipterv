using Stl.CommandR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace Dipterv.Shared.Dto.ProductInventory
{
    public class EditProductInventoryCommand : ICommand<Unit>
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
