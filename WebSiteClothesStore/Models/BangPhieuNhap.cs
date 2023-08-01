namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BangPhieuNhap")]
    public partial class BangPhieuNhap
    {
        [Key]
        public int MaPN { get; set; }

        public int MaNCC { get; set; }

        public DateTime NgayNhap { get; set; }

        public bool Daxoa { get; set; }

        public virtual NhaCC NhaCC { get; set; }
    }
}
