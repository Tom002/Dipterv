using Stl.CommandR;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto.SpecialOffer
{
    public class AddSpecialOfferProductCommand : ICommand<Unit>
    {
        [Required]
        public int SpecialOfferId { get; set; }

        [Required]
        public int ProductId { get; set; }
    }
}
