using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;

namespace WebSiteClothesStore.Controllers
{
    public class TimKiemController : Controller
    {
        // GET: TimKiem
        MydataContext db = new MydataContext();
        [HttpGet]
        public ActionResult KQTimKiem(string tuKhoa, int? page  , FormCollection collection)
        {
            
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            //tìm kiếm theo tên sản phẩm
            var listSP = db.BangSanPhams.Where(n => n.TenSP.Contains(tuKhoa));
            int PageSize = 6;// số sản phẩm trên trang
            // Tạo biến thwu2 số trang hiện tại
            int PageNumber = (page ?? 1);// nếu page không có dữu liệu thì mặc định là 1
            ViewBag.TuKhoa = tuKhoa;
            return View(listSP.OrderBy(n => n.MaSP));
            //return View(listSP.OrderBy(n => n.MaSP).ToPagedList(PageNumber, PageSize));
        }
        [HttpPost] 
        public ActionResult LayTuKhoaTimKiem(string tuKhoa)
        {
            //Gọi về get Tìm kiếm
            return RedirectToAction("KQTimKiem", new { @tuKhoa = tuKhoa }); // gọi về action RedirectToAction
        }
        [HttpPost]
        public ActionResult KQTimKiemPartial(string tuKhoa)
        {
            // tìm kiếm them sản phẩm
            var listSP = db.BangSanPhams.Where(n => n.TenSP.Contains(tuKhoa));
            ViewBag.Tukhoa = tuKhoa;
            return PartialView(listSP.OrderBy(n => n.Dongia));
        }
    }
}