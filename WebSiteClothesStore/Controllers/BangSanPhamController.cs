using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;
using System.IO;
using OfficeOpenXml;

namespace WebSiteClothesStore.Controllers
{
    public class BangSanPhamController : Controller
    {
        MydataContext context = new MydataContext();
        // GET: BangSanPham
        public ActionResult ListSanPham()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var All_sp = context.BangSanPhams;
            return View(All_sp);
        }
        public ActionResult ListProductAdmin()
        {
            //if (Session["TaiKhoanAdmin"] == null)
            //{
            //    return RedirectToAction("DangNhap", "Admin");
            //}
            var All_sp = context.CTSanPhams;
            return View(All_sp);
        }
        public ActionResult Edit(int? id)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            if (id == null)
            {
                return HttpNotFound();
            }
            var listCategory = context.LoaiSanPhams;
            ViewBag.ListLoai = listCategory;
            BangSanPham bsp = context.BangSanPhams.FirstOrDefault(s => s.MaSP == id);
            if (bsp == null)
                return HttpNotFound();
            return View(bsp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BangSanPham bsp)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            
                    var editbsp = context.BangSanPhams.FirstOrDefault(b => b.MaSP == bsp.MaSP);
                    editbsp.TenSP = bsp.TenSP;
                    
                    editbsp.Dongia = bsp.Dongia;
                    editbsp.NgapCapNhat = DateTime.Now;
                    editbsp.Anh1 = bsp.Anh1;
                    editbsp.Anh2 = bsp.Anh2;
                    editbsp.Anh3 = bsp.Anh3;
                    editbsp.Anh4 = bsp.Anh4;
                    editbsp.Anh5 = bsp.Anh5;
                   
                    context.SaveChanges();
                    return View("ListSanPham", context.BangSanPhams.OrderBy(p => p.TenSP));
                           
            //else
            //{
            //    ModelState.AddModelError("", "Input error!");
            //    return View(bsp);
            //}


        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            string newFile = "/Content/images/" + file.FileName;

            if (Directory.Exists(newFile) != true)
            {
                file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            }
            return newFile;
        }

        public ActionResult Create()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var listCategory = context.LoaiSanPhams;
            ViewBag.ListLoai = listCategory;
            return View();
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var tenSP = collection["TenSP"];
            var donGia = collection["Dongia"];
            var MoTa = collection["MoTa"];
            var giamGia = collection["GiamGia"];
            var maLoai = collection["LoaiSanPham"];
            var anh1 = collection["Anh1"];
            var anh2 = collection["Anh2"];
            var anh3 = collection["Anh3"];
            var anh4 = collection["Anh4"];
            var anh5 = collection["Anh5"];

            BangSanPham bangSanPham = new BangSanPham()
            {
                TenSP=tenSP,
                Dongia= decimal.Parse(donGia),
                MoTa = MoTa,
                GiamGia=double.Parse(giamGia!=""?giamGia:"0"),
                MaLoai=int.Parse(maLoai),
                Anh1=anh1,
                Anh2=anh2,
                Anh3=anh3,
                Anh4=anh4,
                Anh5=anh5

            };
            context.BangSanPhams.Add(bangSanPham);
            context.SaveChanges();

            // không được
            return RedirectToAction("ListSanPham");

        }
        public ActionResult Delete(int id)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var D_sp = context.BangSanPhams.FirstOrDefault(m => m.MaSP == id);
            return View(D_sp);
        }
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            /*if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoaiSanPham lsp = context.LoaiSanPhams.Where(m => m.MaLoai == id).First();
            if (lsp == null)
            {
                return HttpNotFound();
            }
            return View(lsp);*/
            var D_sp = context.BangSanPhams.Where(m => m.MaSP == id).First();
            var listCTPro = context.CTSanPhams.Where(p => p.MaSP == D_sp.MaSP);
            // lay ra taat car chi tiet san pham thuoc cai sanr pham
            foreach(var item in listCTPro)
            {
                using (var data = new MydataContext())
                {
                    context.CTSanPhams.Remove(item);
                    // xoa tung chi tiet san pham
                    data.SaveChanges();
                }
            }
            context.BangSanPhams.Remove(D_sp);
            context.SaveChanges();
            return RedirectToAction("ListSanPham");
        }
        public ActionResult Export()
        {
            var data = context.BangSanPhams.ToList();

            var stream = new MemoryStream();
            using (var package = new ExcelPackage(stream))
            {
                var sheet = package.Workbook.Worksheets.Add("Loai");
                sheet.Cells[1, 1].Value ="Mã Sản Phẩm";
                sheet.Cells[1, 2].Value = "Tên Sản Phẩm";
                sheet.Cells[1, 3].Value = "Giá";
                sheet.Cells[1, 4].Value = "Ngày Cập Nhật";
                sheet.Cells[1, 5].Value = "Mã Seal";

                int rowIndex = 2;
                foreach(var item in data)
                {
                    sheet.Cells[rowIndex, 1].Value = item.MaSP;
                    sheet.Cells[rowIndex, 2].Value = item.TenSP;
                    sheet.Cells[rowIndex, 3].Value = item.Dongia;
                    sheet.Cells[rowIndex, 4].Value = item.NgapCapNhat;
                    sheet.Cells[rowIndex, 5].Value = item.GiamGia;
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