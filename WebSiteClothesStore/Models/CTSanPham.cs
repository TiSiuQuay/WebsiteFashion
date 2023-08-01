namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("CTSanPham")]
    public partial class CTSanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CTSanPham()
        {
            CTDonDatHangs = new HashSet<CTDonDatHang>();
        }

        [Key]
        public int MaCT { get; set; }

        [StringLength(50)]
        public string KichThuoc { get; set; }

        public int? SoLuongTon { get; set; }

        public int MaSP { get; set; }

        public bool? Daxoa { get; set; }

        public virtual BangSanPham BangSanPham { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDonDatHang> CTDonDatHangs { get; set; }
    }
}
