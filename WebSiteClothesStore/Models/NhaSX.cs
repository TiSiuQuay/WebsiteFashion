namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("NhaSX")]
    public partial class NhaSX
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public NhaSX()
        {
            BangSanPhams = new HashSet<BangSanPham>();
        }

        [Key]
        public int MaNSX { get; set; }

        [Required]
        [StringLength(50)]
        public string TenNSX { get; set; }

        [StringLength(50)]
        public string ThongTin { get; set; }

        [StringLength(50)]
        public string Logo { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BangSanPham> BangSanPhams { get; set; }
    }
}
