using Stl.CommandR;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto
{
    public class DeleteProductCommand : ICommand<Unit>
    {
        [Required]
        public int ProductId { get; set; }
    }
}
