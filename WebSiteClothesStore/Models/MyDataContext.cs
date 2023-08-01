using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebSiteClothesStore.Models
{
    public partial class MydataContext : DbContext
    {
        public MydataContext()
            : base("name=MydataContext")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<BangPhieuNhap> BangPhieuNhaps { get; set; }
        public virtual DbSet<BangSanPham> BangSanPhams { get; set; }
        public virtual DbSet<CTDonDatHang> CTDonDatHangs { get; set; }
        public virtual DbSet<CTPhieuNhap> CTPhieuNhaps { get; set; }
        public virtual DbSet<CTSanPham> CTSanPhams { get; set; }
        public virtual DbSet<DonDatHang> DonDatHangs { get; set; }
        public virtual DbSet<KhachHang> KhachHangs { get; set; }
        public virtual DbSet<LoaiSanPham> LoaiSanPhams { get; set; }
        public virtual DbSet<LoaiTV> LoaiTVs { get; set; }
        public virtual DbSet<NhaCC> NhaCCs { get; set; }
        public virtual DbSet<NhaSX> NhaSXes { get; set; }
        public virtual DbSet<ThanhVien> ThanhViens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>()
                .Property(e => e.TaiKhoan)
                .IsUnicode(false);

            modelBuilder.Entity<Admin>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);

            modelBuilder.Entity<BangSanPham>()
                .Property(e => e.Dongia)
                .HasPrecision(19, 4);

            modelBuilder.Entity<BangSanPham>()
                .HasMany(e => e.CTPhieuNhaps)
                .WithRequired(e => e.BangSanPham)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<CTDonDatHang>()
                .Property(e => e.DonGia)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CTPhieuNhap>()
                .Property(e => e.MaCTPN)
                .IsUnicode(false);

            modelBuilder.Entity<CTPhieuNhap>()
                .Property(e => e.DonGiaNhap)
                .HasPrecision(19, 4);

            modelBuilder.Entity<CTSanPham>()
                .HasMany(e => e.CTDonDatHangs)
                .WithOptional(e => e.CTSanPham)
                .HasForeignKey(e => e.MaCTSP);

            modelBuilder.Entity<LoaiTV>()
                .HasMany(e => e.ThanhViens)
                .WithRequired(e => e.LoaiTV)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<NhaCC>()
                .HasMany(e => e.BangPhieuNhaps)
                .WithRequired(e => e.NhaCC)
                .WillCascadeOnDelete(false);
        }
    }
}
