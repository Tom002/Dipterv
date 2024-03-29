﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace Dipterv.Dal.Model
{
    [Table("BillOfMaterials", Schema = "Production")]
    [Index(nameof(UnitMeasureCode), Name = "IX_BillOfMaterials_UnitMeasureCode")]
    public partial class BillOfMaterial
    {
        [Key]
        [Column("BillOfMaterialsID")]
        public int BillOfMaterialsId { get; set; }
        [Column("ProductAssemblyID")]
        public int? ProductAssemblyId { get; set; }
        [Column("ComponentID")]
        public int ComponentId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Required]
        [StringLength(3)]
        public string UnitMeasureCode { get; set; }
        [Column("BOMLevel")]
        public short Bomlevel { get; set; }
        [Column(TypeName = "decimal(8, 2)")]
        public decimal PerAssemblyQty { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime ModifiedDate { get; set; }

        [ForeignKey(nameof(ComponentId))]
        [InverseProperty(nameof(Product.BillOfMaterialComponents))]
        public virtual Product Component { get; set; }
        [ForeignKey(nameof(ProductAssemblyId))]
        [InverseProperty(nameof(Product.BillOfMaterialProductAssemblies))]
        public virtual Product ProductAssembly { get; set; }
        [ForeignKey(nameof(UnitMeasureCode))]
        [InverseProperty(nameof(UnitMeasure.BillOfMaterials))]
        public virtual UnitMeasure UnitMeasureCodeNavigation { get; set; }
    }
}
