using Stl.CommandR;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto
{
    public class AddProductReviewCommand : ICommand<Unit>
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [StringLength(50)]
        public string ReviewerName { get; set; }
        [Required]
        [StringLength(50)]
        public string EmailAddress { get; set; }
        public int Rating { get; set; }
        [StringLength(3850)]
        public string Comments { get; set; }
    }
}
