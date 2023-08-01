using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;

namespace WebSiteClothesStore.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        MydataContext context = new MydataContext();
        public ActionResult DangNhap()
        {

            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection , string strURL)
        {
            var taiKhoan = collection["txtTenDangNhap"];
            var matKhau = collection["txtMatKhau"];

            var accountAdmin = context.Admins.FirstOrDefault(p => p.TaiKhoan == taiKhoan && p.MatKhau == matKhau);
            if (accountAdmin== null)
            {
                return Redirect(strURL);
            }
            Session["TaiKhoanAdmin"] = accountAdmin;
            return RedirectToAction("ListDonHangDaGiao", "DonDatHang");

        }
        public ActionResult DangXuat()
        {
            Session["TaiKhoanAdmin"] = null;
            return RedirectToAction("DangNhap", "Admin");
        }
    }
}