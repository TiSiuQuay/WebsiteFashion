namespace WebSiteClothesStore.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Admin")]
    public partial class Admin
    {
        [Key]
        public int MaNV { get; set; }

        [StringLength(50)]
        public string TenNV { get; set; }

        [StringLength(100)]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "E-mail không hợp lệ")]
        public string Email { get; set; }

        [StringLength(12)]
        [RegularExpression("/((09|03|07|08|05)+([0 - 9]{8})\b)/g", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SDT { get; set; }

        [StringLength(20)]
        public string TaiKhoan { get; set; }

        [StringLength(20)]
        public string MatKhau { get; set; }
    }
}
