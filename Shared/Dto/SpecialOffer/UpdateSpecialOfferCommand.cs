using Stl.CommandR;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reactive;

namespace Dipterv.Shared.Dto.SpecialOffer
{
    public class UpdateSpecialOfferCommand : ICommand<Unit>
    {
        [Required]
        public int SpecialOfferId { get; set; }
        [Required]
        [StringLength(255)]
        public string Description { get; set; }
        [Required]
        public decimal DiscountPct { get; set; }
        [Required]
        [StringLength(50)]
        public string Type { get; set; }
        [Required]
        [StringLength(50)]
        public string Category { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public int MinQty { get; set; }
        public int? MaxQty { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
