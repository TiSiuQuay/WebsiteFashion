namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BangSanPham")]
    public partial class BangSanPham
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public BangSanPham()
        {
            CTDonDatHangs = new HashSet<CTDonDatHang>();
            CTPhieuNhaps = new HashSet<CTPhieuNhap>();
            CTSanPhams = new HashSet<CTSanPham>();
        }

        [Key]
        public int MaSP { get; set; }

        [Required]
        [StringLength(50)]
        public string TenSP { get; set; }

        [Column(TypeName = "money")]
        public decimal Dongia { get; set; }

        public DateTime? NgapCapNhat { get; set; }

        public string MoTa { get; set; }

        public int? LuotXem { get; set; }

        public int? LuotBinhChon { get; set; }

        public int? LuotBInhLuan { get; set; }

        public int? SoLanMua { get; set; }

        public double? GiamGia { get; set; }

        public int? MaNCC { get; set; }

        public int MaLoai { get; set; }

        public int? MaNSX { get; set; }

        public bool? DaXoa { get; set; }

        [StringLength(50)]
        public string Anh1 { get; set; }

        [StringLength(50)]
        public string Anh2 { get; set; }

        [StringLength(50)]
        public string Anh3 { get; set; }

        [StringLength(200)]
        public string Anh4 { get; set; }

        [StringLength(200)]
        public string Anh5 { get; set; }

        public virtual LoaiSanPham LoaiSanPham { get; set; }

        public virtual NhaCC NhaCC { get; set; }

        public virtual NhaSX NhaSX { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTDonDatHang> CTDonDatHangs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTPhieuNhap> CTPhieuNhaps { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CTSanPham> CTSanPhams { get; set; }
    }
}
