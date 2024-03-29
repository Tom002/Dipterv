﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Dipterv.Dal.Model
{
    [Table("ProductInventory", Schema = "Production")]
    public partial class ProductInventory
    {
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }
        [Key]
        [Column("LocationID")]
        public short LocationId { get; set; }
        [Required]
        [StringLength(10)]
        public string Shelf { get; set; }
        public byte Bin { get; set; }
        public short Quantity { get; set; }
        [Column("rowguid")]
        public Guid Rowguid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(LocationId))]
        [InverseProperty("ProductInventories")]
        public virtual Location Location { get; set; }
        [ForeignKey(nameof(ProductId))]
        [InverseProperty("ProductInventories")]
        public virtual Product Product { get; set; }
    }
}
