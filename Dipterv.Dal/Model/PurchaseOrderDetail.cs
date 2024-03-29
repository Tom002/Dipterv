﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Dipterv.Dal.Model
{
    [Table("PurchaseOrderDetail", Schema = "Purchasing")]
    [Index(nameof(ProductId), Name = "IX_PurchaseOrderDetail_ProductID")]
    public partial class PurchaseOrderDetail
    {
        [Key]
        [Column("PurchaseOrderID")]
        public int PurchaseOrderId { get; set; }
        [Key]
        [Column("PurchaseOrderDetailID")]
        public int PurchaseOrderDetailId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime DueDate { get; set; }
        public short OrderQty { get; set; }
        [Column("ProductID")]
        public int ProductId { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal LineTotal { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal ReceivedQty { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal RejectedQty { get; set; }
        [Column(TypeName = "decimal(9, 2)")]
        public decimal StockedQty { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(ProductId))]
        [InverseProperty("PurchaseOrderDetails")]
        public virtual Product Product { get; set; }
        [ForeignKey(nameof(PurchaseOrderId))]
        [InverseProperty(nameof(PurchaseOrderHeader.PurchaseOrderDetails))]
        public virtual PurchaseOrderHeader PurchaseOrder { get; set; }
    }
}
