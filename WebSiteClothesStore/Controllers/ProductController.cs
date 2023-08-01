using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;
using PagedList;

namespace WebsiteStoreClothes.Controllers
{

    public class ProductController : Controller
    {
        MydataContext db = new MydataContext();
        int idMaLoai;
        // GET: Product
        public ActionResult ShowAllProduct(int? page)
        {
           
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int PageSize = 6;// số sản phẩm trên trang
            // Tạo biến thwu2 số trang hiện tại
            int PageNumber = (page ?? 1);// nếu page không có dữu liệu thì mặc định là 1
            var listSP = db.BangSanPhams;
            return View(listSP.OrderBy(n => n.Dongia).ToPagedList(PageNumber, PageSize));
        }
        public ActionResult Products(int? page)
        {
            // Thực hiện chức năng phân trang
            //Tạo biến số sản phẩm trên trang
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int PageSize = 6;// số sản phẩm trên trang
            // Tạo biến thwu2 số trang hiện tại
            int PageNumber = (page ?? 1);// nếu page không có dữu liệu thì mặc định là 1
            var listSP = db.BangSanPhams;
            return View(listSP.OrderBy(n => n.Dongia).ToPagedList(PageNumber, PageSize));
        }
        public ActionResult ProductFollowIDCategory(int? idCategory ,int? page)
        {
            if (idCategory == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            var listProduct = db.BangSanPhams.Where(p => p.MaLoai == idCategory);
            if (listProduct.Count() == 0)
            {
                return HttpNotFound();
            }
            if (Request.HttpMethod != "GET")
            {
                page = 1;
            }
            int pageSize = 6;
            // số sản phẩm trên trang
            // Tạo biến thwu2 số trang hiện tại
            int pageNumber = (page ?? 1);// nếu page không có dữu liệu thì mặc định là 1
            Session["MaLoai"]= (int)idCategory;
            idMaLoai = (int)idCategory;
            ViewBag.MaLoaiSP = idCategory;
            ViewBag.LoaiSP = db.LoaiSanPhams.FirstOrDefault(p => p.MaLoai == idCategory).TenLoai;
            return View(listProduct.OrderBy(p=>p.Dongia).ToPagedList(pageNumber,pageSize));
        }
        public ActionResult CategoryProduct()
        {
            var listCategor = db.LoaiSanPhams;
            if(Session["MaLoai"]!=null)
                ViewBag.MaLoai = Session["MaLoai"];
            return PartialView(listCategor);
        }
        public ActionResult DetailProduct(int? maSP)
        {

            if (maSP == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            }
            var product = db.BangSanPhams.FirstOrDefault(p => p.MaSP == maSP);
            if (product == null)
            {
                return HttpNotFound();
            }
            var listSameCategoryProduct = db.BangSanPhams.Where(p => p.MaLoai == product.MaLoai);
            product.LuotXem++;
            db.SaveChanges();
            ViewBag.ListPro = listSameCategoryProduct;
            ViewBag.Product = product;
            return View();
        }
    }
}