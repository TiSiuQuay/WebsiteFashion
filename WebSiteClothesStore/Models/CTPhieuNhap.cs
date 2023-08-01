namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CTPhieuNhap")]
    public partial class CTPhieuNhap
    {
        [Key]
        [StringLength(50)]
        public string MaCTPN { get; set; }

        public int MaSP { get; set; }

        [Column(TypeName = "money")]
        public decimal? DonGiaNhap { get; set; }

        public int? SoLuongNhap { get; set; }

        public virtual BangSanPham BangSanPham { get; set; }
    }
}
