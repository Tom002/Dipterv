﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Dipterv.Dal.Model
{
    [Table("WorkOrderRouting", Schema = "Production")]
    [Index(nameof(ProductId), Name = "IX_WorkOrderRouting_ProductID")]
    public partial class WorkOrderRouting
    {
        [Key]
        [Column("WorkOrderID")]
        public int WorkOrderId { get; set; }
        [Key]
        [Column("ProductID")]
        public int ProductId { get; set; }
        [Key]
        public short OperationSequence { get; set; }
        [Column("LocationID")]
        public short LocationId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ScheduledStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ScheduledEndDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualStartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualEndDate { get; set; }
        [Column(TypeName = "decimal(9, 4)")]
        public decimal? ActualResourceHrs { get; set; }
        [Column(TypeName = "money")]
        public decimal PlannedCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? ActualCost { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(LocationId))]
        [InverseProperty("WorkOrderRoutings")]
        public virtual Location Location { get; set; }
        [ForeignKey(nameof(WorkOrderId))]
        [InverseProperty("WorkOrderRoutings")]
        public virtual WorkOrder WorkOrder { get; set; }
    }
}
