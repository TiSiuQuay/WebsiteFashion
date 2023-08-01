using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSiteClothesStore.Models;

namespace WebSiteClothesStore.ModelTemp
{
    public class ItemCardTemp
    {
        public int MaCTDDH { get; set; }
        public int MaSP { get; set; }
        public int MaCT { get; set; }
        public string TenSP { get; set; }
        public string Size { get; set; }
        public int SoLuong { get; set; }
        public double DonGia { get; set; }
        public double ThanhTien { get; set; }
        public string HinhAnh { get; set; }

        public ItemCardTemp()
        {

        }
        public ItemCardTemp(int? maSP,int? maCT)
        {
            using (MydataContext db = new MydataContext())
            {
                BangSanPham spMua = db.BangSanPhams.Single(n => n.MaSP == maSP);
                CTSanPham ctSP = db.CTSanPhams.Single(p => p.MaCT == maCT);
                this.MaCTDDH++;
                this.MaSP = (int)maSP;
                this.MaCT = (int)maCT;
                this.Size = ctSP.KichThuoc;
                this.TenSP = spMua.TenSP;
                this.HinhAnh = spMua.Anh1;
                this.DonGia = (double)spMua.Dongia;
                this.SoLuong = 1;
                this.ThanhTien = (double)(spMua.Dongia * SoLuong);
            }
        }
    }
}