using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Models;
using WebSiteClothesStore.ModelTemp;
using WebSiteClothesStore.LinqToSQL;
using System.Net.Mail;
using System.Text;
using WebSiteClothesStore.Mail;

namespace WebSiteClothesStore.Controllers
{
    public class HomeController : Controller
    {
        MydataContext db = new MydataContext();
        DataContextLinqDataContext data = new DataContextLinqDataContext();
        public ActionResult Index()
        {
            var listLoaiSanPham = db.LoaiSanPhams.Take(3);
            ViewBag.ListCategory = listLoaiSanPham;
            var listProduct = db.BangSanPhams.OrderBy(p=>p.SoLanMua).Take(6);
            var listDetailProduct = db.CTSanPhams.Take(8);
            ViewBag.ListProduct = listProduct;
            return View();
        }

        public ActionResult MenuPartial()
        {
            var listSP = db.BangSanPhams;
            // truy vấn vào bảng sản phẩm để có thể tham chiếu vào các thuộc tính khóa ngoại khác không cần truy vấn nhìu bảng
            return PartialView(listSP); // dùng PartialView khi chỉ tả về 1 phần nhỏ trong layout
        }
        // xây dựng action
        // đang nhập
    
        [HttpPost]
        public ActionResult DangNhap(string strURL, FormCollection f)
        {
            //Kiểm tra tên đưng nhập và mật khẩu
            string taiKhoan = f["txtTenDangNhap"].ToString();// muốn nhận giá trị thì nhận theo name
            string matKhau = f["txtMatKhau"].ToString();

            Models.ThanhVien tv = db.ThanhViens.SingleOrDefault(n => n.TaiKhoan == taiKhoan && n.MatKhau == matKhau);
            if (tv != null)
            {
                Models.KhachHang kh = db.KhachHangs.FirstOrDefault(p => p.MaTV == tv.MaTV);
                Models.DonDatHang cartOfClient = db.DonDatHangs.FirstOrDefault(p => p.MaKH == kh.MaKH && p.DaDat == false);
                //Lấy ra đơn hàng mà khách chưa đặt
                if (Session["GioHangTam"] != null)// đã đặt
                {
                    // lấy ra đơn hàng khách chưa đặt
                    if (cartOfClient != null)// nếu khách hàng đó đã có giỏ hàng
                    {
                            List<ItemCardTemp> listItemCard = Session["GioHangTam"] as List<ItemCardTemp>;  // lấy ra danh sách giỏ hàng đang có
                            foreach (var item in listItemCard) // lấy ra từng sản phẩm có trong list
                            {
                                LinqToSQL.CTDonDatHang productExist = data.CTDonDatHangs.SingleOrDefault(p => p.MaCTSP == item.MaCT && item.MaSP == p.MaSP && p.MaDDH == cartOfClient.MaDDH);
                                if (productExist != null) // nếu đã có sản phẩm
                                {
                                    productExist.SoLuong += item.SoLuong;
                                    UpdateModel(productExist);
                                    data.SubmitChanges();
                                }
                                else
                                {
                                    using (var context = new MydataContext())
                                    {
                                        Models.CTDonDatHang ctDonDatHang = new Models.CTDonDatHang();
                                        ctDonDatHang.MaDDH = cartOfClient.MaDDH;// cập nhật lại đơn hàng của khách
                                        ctDonDatHang.MaSP = item.MaSP;
                                        ctDonDatHang.TenSP = item.TenSP;
                                        ctDonDatHang.SoLuong = item.SoLuong;
                                        ctDonDatHang.DonGia = (decimal)item.ThanhTien;
                                        ctDonDatHang.MaCTSP = item.MaCT;
                                        context.CTDonDatHangs.Add(ctDonDatHang);
                                        context.SaveChanges();
                                    }
                                }
                            }
                    }

                    else // chưa có đơn nào 
                    {
                        Models.DonDatHang donDatHang = new Models.DonDatHang();
                        donDatHang.DaDat = false;
                        donDatHang.MaKH= kh.MaKH;
                        db.DonDatHangs.Add(donDatHang);
                        db.SaveChanges();
                        
                            List<ItemCardTemp> listItemCard = Session["GioHangTam"] as List<ItemCardTemp>;  // lấy ra danh sách giỏ hàng đang có
                            foreach (var item in listItemCard) // lấy ra từng sản phẩm có trong list
                            {
                                using (var context = new MydataContext())
                                {
                                    Models.CTDonDatHang ctDonDatHang = new Models.CTDonDatHang();
                                    ctDonDatHang.MaDDH = donDatHang.MaDDH;// cập nhật lại đơn hàng của khách
                                    ctDonDatHang.MaSP = item.MaSP;
                                    ctDonDatHang.TenSP = item.TenSP;
                                    ctDonDatHang.SoLuong = item.SoLuong;
                                    ctDonDatHang.DonGia = (decimal)item.DonGia;
                                    ctDonDatHang.MaCTSP = item.MaCT;
                                    context.CTDonDatHangs.Add(ctDonDatHang);
                                    context.SaveChanges();
                                }
                            }
                        Session["GioHangTam"] = null;

                        Session["GioHang"] = donDatHang;
                    }
                   

                    // cập nhật lại thông tin khách hàng
                }
                if (cartOfClient != null)// nếu có đơn hàng chưa đặt
                    Session["GioHang"] = cartOfClient;
                Session["TaiKhoan"] = tv;
                //return RedirectToAction("Index");// chuyển hướng trang thông qua view truy cập vào action rồi chuyển hướng tới view
                return Redirect(strURL); // reload lại trang đang hiển thị
            }
            return Content("Tài khoản hoặc mật khẩu không đúng");// chuyển hướng trang thông qua view truy cập vào action rồi chuyển hướng tới view

        }
        public ActionResult DangXuat() 
        {
            Session["TaiKhoan"] = null;
            Session["KhachHang"] = null;
            Session["GioHang"] = null;
            Session["GioHangTam"] = null;
            ViewBag.GioHangCSDL = null;
            return RedirectToAction("Index");
        }
        public ActionResult LayMayKhau(string strURL,FormCollection collection) 
        {
            string taiKhoan = collection["TaiKhoan"];
            string email = collection["Email"];
            Random rd = new Random();
            Random ranDom = new Random();
            string newPassword="";
            var account = db.ThanhViens.FirstOrDefault(p => p.TaiKhoan == taiKhoan);
            if (account != null)
            {
                for (int i=0; i< 4; i++)
                {

                    newPassword += Convert.ToString((char)rd.Next(97, 122))+ ranDom.Next(0,9);

                }
          
                account.MatKhau = newPassword;
                account.Email = email;

                db.SaveChanges();

                MailMessage mSG = new MailMessage();
                AlternateView plainView = AlternateView
    .CreateAlternateViewFromString("Some plaintext", Encoding.UTF8, "text/plain");
                mSG.AlternateViews.Add(plainView);
                mSG.From = new MailAddress(MyEmail.name, "Thông báo nhận lại mật khẩu từ shop");
                mSG.To.Add(email); // thêm địa chỉ mail người nhận
                mSG.Subject = "Mật khẩu vừa cập nhật, vui lòng đăng nhập lại";// Thêm tiêu đề mail;
                string style = "color:red; font-size:20px; font-weight:700;";
                string test = string.Format(@"""{0}""", style);
                string htmlText = $"<b>Mật khẩu mới của bạn là:</b> <span style={test}> "+ newPassword +"</span>";

                AlternateView htmlView =
                    AlternateView.CreateAlternateViewFromString(htmlText, Encoding.UTF8, "text/html");

                mSG.AlternateViews.Add(htmlView);
                mSG.Body = htmlText;
                mSG.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();

                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.EnableSsl = true;
                smtp.Credentials = new System.Net.NetworkCredential(MyEmail.name, MyEmail.password);
                smtp.Send(mSG);// gửi
                mSG = null;

            }
            return RedirectToAction("Index");
        }
        public ActionResult DangKy(FormCollection collection , string strURL)
        {
            string taiKhoan = collection["TaiKhoan"];
            string matKhau = collection["MatKhau"];
            string nhapLaiMK = collection["reMatKhau"];
            string hoTen = collection["HoTen"];
            string email = collection["Email"];
            string sDT = collection["SDT"];
            string diaChi = collection["DiaChi"];
            string cauHoi = collection["CauHoi"];

            Models.ThanhVien newMember = new Models.ThanhVien();
            newMember.MaLoaiTV = 1;
            newMember.HoTen = hoTen;
            newMember.MatKhau = matKhau;
            newMember.TaiKhoan = taiKhoan;
            newMember.SDT = sDT;
            newMember.Email = email;
            newMember.CauHoi = cauHoi;
            newMember.DiaChi = diaChi.Trim(); ;

            db.ThanhViens.Add(newMember);
            db.SaveChanges();


            Models.KhachHang newClient = new Models.KhachHang();
            newClient.MaTV = newMember.MaTV;
            newClient.SDT = sDT;
            newClient.Email = email;
            newClient.DiaChi = diaChi.Trim();
            newClient.TenKH = hoTen;

            db.KhachHangs.Add(newClient);
            db.SaveChanges();
            Session["TaiKhoan"] = newMember;
            return Redirect(strURL);

        }
        
        public ActionResult ShowCardUser()
        {
            if (Session["TaiKhoan"] != null)
            {
                Models.ThanhVien member = Session["TaiKhoan"] as Models.ThanhVien;
                Models.KhachHang client = db.KhachHangs.FirstOrDefault(p => p.MaTV == member.MaTV);
                var listCardClient = db.DonDatHangs.Where(p => p.MaKH == client.MaKH && p.DaDat==true);
                return View(listCardClient);
            }
            if (Session["KhachHang"] != null)
            {
                Models.KhachHang kh = Session["KhachHang"] as Models.KhachHang;
                var listCardClient = db.DonDatHangs.Where(p => p.MaKH == kh.MaKH && p.DaDat == true);
                return View(listCardClient);
            }
            return View("ShowCardUser");
        }
        public ActionResult DetailCard(int maDH)
        {
            var listCard = db.CTDonDatHangs.Where(p => p.MaDDH == maDH);
            return View(listCard);
        }
        public ActionResult DestroyCard(int maDH)
        {
            var donHang = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maDH);
            donHang.DaHuy =true;
            donHang.TinhTrangDDH = "Khách không nhận hàng";
            db.SaveChanges();
            return RedirectToAction("ShowCardUser");
        }
        public ActionResult ReOrderCard(int maDH)
        {
            var donHang = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maDH);
            donHang.DaHuy = false;
            donHang.TinhTrangDDH = "Đang giao";
            db.SaveChanges();
            return RedirectToAction("ShowCardUser");
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}