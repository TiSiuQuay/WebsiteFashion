namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CTDonDatHang")]
    public partial class CTDonDatHang
    {
        [Key]
        public int MaCTDDH { get; set; }

        public int MaDDH { get; set; }

        [StringLength(50)]
        public string TenSP { get; set; }

        public int SoLuong { get; set; }

        [Column(TypeName = "money")]
        public decimal DonGia { get; set; }

        public int? BinhChon { get; set; }

        public int? MaSP { get; set; }

        public int? MaCTSP { get; set; }

        public virtual BangSanPham BangSanPham { get; set; }

        public virtual CTSanPham CTSanPham { get; set; }

        public virtual DonDatHang DonDatHang { get; set; }
    }
}
