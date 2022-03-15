using Stl.CommandR;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto
{
    public class AddProductCommand : ICommand<Unit>
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(25)]
        public string ProductNumber { get; set; }
        public decimal StandardCost { get; set; }
        public decimal ListPrice { get; set; }
        [StringLength(5)]
        public string Size { get; set; }
    }
}
