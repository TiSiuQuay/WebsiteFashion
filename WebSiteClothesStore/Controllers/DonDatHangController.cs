using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;

namespace WebSiteClothesStore.Controllers
{
    public class DonDatHangController : Controller
    {
        MydataContext context = new MydataContext();
        // GET: DonDatHang
        public ActionResult ListDonHangDaGiao()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var listDonHang = context.DonDatHangs.Where(p=>p.DaDat==true && p.DaThanhToan==true && p.DaHuy!=true && p.TinhTrangDDH.Contains("Đã giao")).OrderBy(p => p.NgayDat);
            return View(listDonHang);
        }
        public ActionResult Export()
        {
            var tongTienDaThanhToan = context.CTDonDatHangs.Where(p => p.DonDatHang.DaThanhToan == true && p.DonDatHang.DaDat == true).Sum(p => p.DonGia * p.SoLuong);
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaThanhToan == true && p.DaHuy != true && p.TinhTrangDDH.Contains("Đã giao")).OrderBy(p => p.NgayDat);
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value = "Tên KH";
                sheet.Cells[1, 2].Value = "SDT";
                sheet.Cells[1, 3].Value = "Địa Chỉ";
                sheet.Cells[1, 4].Value = "Email";
                sheet.Cells[1, 5].Value = "TTDDH";
                sheet.Cells[1, 6].Value = "Đã Thanh Toán";
                sheet.Cells[1, 7].Value = "Ưu Đãi";
                sheet.Cells[1, 8].Value = "Ngày Đặt";
                sheet.Cells[1, 9].Value = "Ngày Giao";
                sheet.Cells[1, 10].Value = "Tổng Tiền";
                int rowIndex = 2;
                foreach (var item in listDonHang)
                {
                    sheet.Cells[rowIndex, 1].Value = item.KhachHang.TenKH;
                    sheet.Cells[rowIndex, 2].Value = item.KhachHang.SDT;
                    sheet.Cells[rowIndex, 3].Value = item.KhachHang.DiaChi;
                    sheet.Cells[rowIndex, 4].Value = item.KhachHang.Email;
                    sheet.Cells[rowIndex, 5].Value = item.TinhTrangDDH;
                    sheet.Cells[rowIndex, 6].Value = item.DaThanhToan==true? "Đã thanh toán": "Chưa thanh toán";
                    sheet.Cells[rowIndex, 7].Value = item.UuDai;
                    sheet.Cells[rowIndex, 8].Value = item.NgayDat.ToString();
                    sheet.Cells[rowIndex, 9].Value = item.NgayGiao.ToString();
                    sheet.Cells[rowIndex, 10].Value = context.CTDonDatHangs.Where(p=>p.MaDDH==item.MaDDH).Sum(n=>n.SoLuong*n.DonGia);
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Loai_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        //
        public ActionResult ListDonHangDangGiao()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaThanhToan != true &&(p.TinhTrangDDH.Contains("Đang giao")||p.TinhTrangDDH.Contains("Khách không nhận hàng"))).OrderBy(p=>p.NgayDat);
            return View(listDonHang);
        }
        public ActionResult Export1()
        {
            var tongTienDaThanhToan = context.CTDonDatHangs.Where(p => p.DonDatHang.DaThanhToan == false && p.DonDatHang.DaDat == true).Sum(p => p.DonGia * p.SoLuong);
            
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaThanhToan != true && p.DaHuy != true).OrderBy(p => p.NgayDat);
            
            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value = "Tên KH";
                sheet.Cells[1, 2].Value = "SDT";
                sheet.Cells[1, 3].Value = "Địa Chỉ";
                sheet.Cells[1, 4].Value = "Email";
                sheet.Cells[1, 5].Value = "TTDDH";
                sheet.Cells[1, 6].Value = "Đã Thanh Toán";
                sheet.Cells[1, 7].Value = "Ưu Đãi";
                sheet.Cells[1, 8].Value = "Ngày Đặt";
                sheet.Cells[1, 9].Value = "Ngày Giao";
                sheet.Cells[1, 10].Value = "Tổng Tiền";
                int rowIndex = 2;
                foreach (var item in listDonHang )
                {
                    sheet.Cells[rowIndex, 1].Value = item.KhachHang.TenKH;
                    sheet.Cells[rowIndex, 2].Value = item.KhachHang.SDT;
                    sheet.Cells[rowIndex, 3].Value = item.KhachHang.DiaChi;
                    sheet.Cells[rowIndex, 4].Value = item.KhachHang.Email;
                    sheet.Cells[rowIndex, 5].Value = item.TinhTrangDDH;
                    sheet.Cells[rowIndex, 6].Value = item.DaThanhToan;
                    sheet.Cells[rowIndex, 7].Value = item.UuDai;
                    sheet.Cells[rowIndex, 8].Value = item.NgayDat.ToString();
                    sheet.Cells[rowIndex, 9].Value = item.NgayDat;
                    sheet.Cells[rowIndex, 8].Value = tongTienDaThanhToan;
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Loai_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        //
        public ActionResult ListDonHangDaThanhToan()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaThanhToan == true && p.DaHuy != true ).OrderBy(p => p.NgayDat);
            return View(listDonHang);
        }
        public ActionResult Export2()
        {
            var tongTienDaThanhToan = context.CTDonDatHangs.Where(p => p.DonDatHang.DaThanhToan == false && p.DonDatHang.DaDat == true).Sum(p => p.DonGia * p.SoLuong);
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaThanhToan == true && p.DaHuy != true).OrderBy(p => p.NgayDat);

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value = "Tên KH";
                sheet.Cells[1, 2].Value = "SDT";
                sheet.Cells[1, 3].Value = "Địa Chỉ";
                sheet.Cells[1, 4].Value = "Email";
                sheet.Cells[1, 5].Value = "TTDDH";
                sheet.Cells[1, 6].Value = "Đã Thanh Toán";
                sheet.Cells[1, 7].Value = "Ưu Đãi";
                sheet.Cells[1, 8].Value = "Ngày Đặt";
                sheet.Cells[1, 9].Value = "Ngày Giao";
                sheet.Cells[1, 10].Value = "Tổng Tiền";
                int rowIndex = 2;
                foreach (var item in listDonHang)
                {
                    sheet.Cells[rowIndex, 1].Value = item.KhachHang.TenKH;
                    sheet.Cells[rowIndex, 2].Value = item.KhachHang.SDT;
                    sheet.Cells[rowIndex, 3].Value = item.KhachHang.DiaChi;
                    sheet.Cells[rowIndex, 4].Value = item.KhachHang.Email;
                    sheet.Cells[rowIndex, 5].Value = item.TinhTrangDDH;
                    sheet.Cells[rowIndex, 6].Value = item.DaThanhToan;
                    sheet.Cells[rowIndex, 7].Value = item.UuDai;
                    sheet.Cells[rowIndex, 8].Value = item.NgayDat.ToString();
                    sheet.Cells[rowIndex, 9].Value = item.NgayDat;
                    sheet.Cells[rowIndex, 8].Value = tongTienDaThanhToan;
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Loai_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        //
        public ActionResult ListDonDatHangDaHuy()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaHuy == true).OrderBy(p => p.NgayDat);
            return View(listDonHang);
        }
        public ActionResult Export3()
        {
            var tongTienDaThanhToan = context.CTDonDatHangs.Where(p => p.DonDatHang.DaThanhToan == false && p.DonDatHang.DaDat == true).Sum(p => p.DonGia * p.SoLuong);
            var listDonHang = context.DonDatHangs.Where(p => p.DaDat == true && p.DaHuy == true).OrderBy(p => p.NgayDat);

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value = "Tên KH";
                sheet.Cells[1, 2].Value = "SDT";
                sheet.Cells[1, 3].Value = "Địa Chỉ";
                sheet.Cells[1, 4].Value = "Email";
                sheet.Cells[1, 5].Value = "TTDDH";
                sheet.Cells[1, 6].Value = "Đã Thanh Toán";
                sheet.Cells[1, 7].Value = "Ưu Đãi";
                sheet.Cells[1, 8].Value = "Ngày Đặt";
                sheet.Cells[1, 9].Value = "Ngày Giao";
                sheet.Cells[1, 10].Value = "Tổng Tiền";
                int rowIndex = 2;
                foreach (var item in listDonHang)
                {
                    sheet.Cells[rowIndex, 1].Value = item.KhachHang.TenKH;
                    sheet.Cells[rowIndex, 2].Value = item.KhachHang.SDT;
                    sheet.Cells[rowIndex, 3].Value = item.KhachHang.DiaChi;
                    sheet.Cells[rowIndex, 4].Value = item.KhachHang.Email;
                    sheet.Cells[rowIndex, 5].Value = item.TinhTrangDDH;
                    sheet.Cells[rowIndex, 6].Value = item.DaThanhToan;
                    sheet.Cells[rowIndex, 7].Value = item.UuDai;
                    sheet.Cells[rowIndex, 8].Value = item.NgayDat.ToString();
                    sheet.Cells[rowIndex, 9].Value = item.NgayDat;
                    sheet.Cells[rowIndex, 8].Value = tongTienDaThanhToan;
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Loai_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public ActionResult CapNhatDonDat(int maDDH , bool status)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var donHang = context.DonDatHangs.FirstOrDefault(p => p.MaDDH == maDDH);
            if (donHang != null)
            {
                donHang.TinhTrangDDH = status == true ? "Đã giao" : "Khách không nhận hàng";
                donHang.DaThanhToan = status;
                donHang.DaHuy = status == false ? true :false;
                context.SaveChanges();
            }
            return Content(status.ToString());
        }

        [HttpGet]

        public ActionResult DetailDHH (int id)
        {
            var donDatHang = context.DonDatHangs.FirstOrDefault(p => p.MaDDH == id);

            var listCTDonDatHang = context.CTDonDatHangs.Where(p => p.MaDDH == id);

            ViewBag.CTDonDatHang = listCTDonDatHang;

            return View(donDatHang);
        }

        public ActionResult DeleteDDH(int id)
        {
            var listDetailDonHang = context.CTDonDatHangs.Where(p=>p.MaDDH==id);

            foreach(var item in listDetailDonHang)
            {
                var itemPro = context.CTSanPhams.FirstOrDefault(p => p.MaCT == item.MaCTSP);
                if (itemPro != null)
                {
                    itemPro.SoLuongTon += item.SoLuong;
                    context.SaveChanges();
                }
            }
            var donDatHang = context.DonDatHangs.FirstOrDefault(p => p.MaDDH == id);
            context.DonDatHangs.Remove(donDatHang);
            context.SaveChanges();
            return RedirectToAction("ListDonDatHangDaHuy", "DonDatHang");
        }
    }
}