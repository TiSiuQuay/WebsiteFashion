using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;
namespace WebSiteClothesStore.Controllers
{
    public class DanhMucController : Controller
    {
        MydataContext context = new MydataContext();
        // GET: DanhMuc
        public ActionResult ListDanhMuc()
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var All_dm = context.LoaiSanPhams;
            return View(All_dm);
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
            LoaiSanPham lsp = context.LoaiSanPhams.FirstOrDefault(s => s.MaLoai == id);
            if (lsp == null)
                return HttpNotFound();
            return View(lsp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoaiSanPham lsp)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var editlsp = context.LoaiSanPhams.FirstOrDefault(b => b.MaLoai == lsp.MaLoai);
                    editlsp.TenLoai = lsp.TenLoai;
                    editlsp.Logo = lsp.Logo;
                    editlsp.BiDanh = lsp.BiDanh;
                    context.SaveChanges();
                    return View("ListDanhMuc", context.LoaiSanPhams);
                }
                catch (Exception)
                {
                    return HttpNotFound();
                }

            }
            else
            {
                ModelState.AddModelError("", "Input error!");
                return View(lsp);
            }

        }
        public string ProcessUpload(HttpPostedFileBase file)
        {
           
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/Content/images/" + file.FileName));
            return "/Content/images/" + file.FileName;
        }
        public ActionResult Delete(int id)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            var D_dm = context.LoaiSanPhams.FirstOrDefault(m => m.MaLoai == id);
            return View(D_dm);
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
            var D_dm = context.LoaiSanPhams.Where(m => m.MaLoai == id).First();
            context.LoaiSanPhams.Remove(D_dm);
            context.SaveChanges();
            return RedirectToAction("ListDanhMuc");
        }
        public ActionResult Create(LoaiSanPham lspCreate)
        {
            if (Session["TaiKhoanAdmin"] == null)
            {
                return RedirectToAction("DangNhap", "Admin");
            }
            if (ModelState.IsValid)
            {

                LoaiSanPham lsp = lspCreate;
                var listlsp = context.LoaiSanPhams.ToList();
                var newId = listlsp.Max(n => n.MaLoai) + 1;
                lsp.MaLoai = newId;
                context.LoaiSanPhams.Add(lsp);
                context.SaveChanges();
                
                return RedirectToAction("ListDanhMuc");
            }
            else
            {
                // không được
                return View(new LoaiSanPham());
            }
        }
    }
}