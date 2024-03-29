﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Dipterv.Dal.Model
{
    [Table("SalesOrderDetail", Schema = "Sales")]
    [Index(nameof(Rowguid), Name = "AK_SalesOrderDetail_rowguid", IsUnique = true)]
    [Index(nameof(ProductId), Name = "IX_SalesOrderDetail_ProductID")]
    public partial class SalesOrderDetail
    {
        [Key]
        [Column("SalesOrderID")]
        public int SalesOrderId { get; set; }
        [Key]
        [Column("SalesOrderDetailID")]
        public int SalesOrderDetailId { get; set; }
        [StringLength(25)]
        public string CarrierTrackingNumber { get; set; }
        public short OrderQty { get; set; }
        [Column("ProductID")]
        public int ProductId { get; set; }
        [Column("SpecialOfferID")]
        public int SpecialOfferId { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }
        [Column(TypeName = "money")]
        public decimal UnitPriceDiscount { get; set; }
        [Column(TypeName = "numeric(38, 6)")]
        public decimal LineTotal { get; set; }
        [Column("rowguid")]
        public Guid Rowguid { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(SalesOrderId))]
        [InverseProperty(nameof(SalesOrderHeader.SalesOrderDetails))]
        public virtual SalesOrderHeader SalesOrder { get; set; }
        [ForeignKey("SpecialOfferId,ProductId")]
        [InverseProperty("SalesOrderDetails")]
        public virtual SpecialOfferProduct SpecialOfferProduct { get; set; }
    }
}
