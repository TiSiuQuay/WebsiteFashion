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
    public class ThanhVienController : Controller
    {
        MydataContext context = new MydataContext();
        // GET: ThanhVien
        public ActionResult ListTV()
        {
            var All_tv = context.ThanhViens;
            return View(All_tv);
        }
        public ActionResult Edit(int? id)
        {
            /*if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }*/
            if (id == null)
            {
                return HttpNotFound();
            }
            ThanhVien tv = context.ThanhViens.FirstOrDefault(s => s.MaTV == id);
            if (tv == null)
                return HttpNotFound();
            return View(tv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ThanhVien bsp)
        {
            /*if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }*/
            if (ModelState.IsValid)
            {
                try
                {
                    var editbsp = context.ThanhViens.FirstOrDefault(b => b.MaTV == bsp.MaTV);
                    editbsp.TaiKhoan = bsp.TaiKhoan;
                    editbsp.MatKhau = bsp.MatKhau;
                    editbsp.HoTen = bsp.HoTen;
                    editbsp.DiaChi = bsp.DiaChi;
                    editbsp.Email = bsp.Email;
                    editbsp.SDT = bsp.SDT;
                    editbsp.CauHoi = bsp.CauHoi;

                    context.SaveChanges();
                    return View("ListTV", context.ThanhViens);
                }
                catch (Exception)
                {
                    return HttpNotFound();
                }

            }
            else
            {
                ModelState.AddModelError("", "Input error!");
                return View(bsp);
            }

        }
        public ActionResult Delete(int id)
        {
            var D_sach = context.ThanhViens.First(m => m.MaTV == id);
            return View(D_sach);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            var D_sach = context.ThanhViens.Where(m => m.MaTV == id).First();
            context.ThanhViens.Remove(D_sach);
            context.SaveChanges();
            return RedirectToAction("ListTV");
        }
        public ActionResult Create()
        {
            /*if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }*/
            var listCategory = context.ThanhViens;
            ViewBag.ListTV = listCategory;
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            /*if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }*/
            
            var tk = collection["TaiKhoan"];
            var mk = collection["MatKhau"];
            var hoten = collection["HoTen"];
            var diachi = collection["DiaChi"];
            var email = collection["Email"];
            var sdt = collection["SDT"];
            var cauhoi = collection["CauHoi"];


            ThanhVien tv = new ThanhVien()
            {
                TaiKhoan = tk,
                MatKhau = mk,
                HoTen = hoten,
                DiaChi = diachi,
                Email = email,
                SDT = sdt,
                CauHoi = cauhoi

            };
            context.ThanhViens.Add(tv);
            context.SaveChanges();

            // không được
            return RedirectToAction("ListTV");

        }

        public ActionResult Export()
        {
            var data = context.ThanhViens.ToList();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value = "Mã Thành VIên";
                sheet.Cells[1, 2].Value = "Tài Khoản";
                sheet.Cells[1, 3].Value = "Họ Tên";
                sheet.Cells[1, 4].Value = "Địa Chỉ";
                sheet.Cells[1, 5].Value = "Email";
                sheet.Cells[1, 6].Value = "SDT";
                sheet.Cells[1, 7].Value = "Câu Hỏi";
                sheet.Cells[1, 8].Value = "Mã Loại Thành Viên";


                int rowIndex = 2;
                foreach (var item in data)
                {
                    sheet.Cells[rowIndex, 1].Value = item.MaTV;
                    sheet.Cells[rowIndex, 2].Value = item.TaiKhoan;
                    sheet.Cells[rowIndex, 3].Value = item.HoTen;
                    sheet.Cells[rowIndex, 4].Value = item.DiaChi;
                    sheet.Cells[rowIndex, 5].Value = item.Email;
                    sheet.Cells[rowIndex, 6].Value = item.SDT;
                    sheet.Cells[rowIndex, 7].Value = item.CauHoi;
                    sheet.Cells[rowIndex, 8].Value = item.MaLoaiTV;
                    rowIndex++;
                }
                package.Save();
            }
            stream.Position = 0;
            var fileName = $"Loai_{DateTime.Now.ToString("yyyyMMddHHmmss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}