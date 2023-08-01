using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebSiteClothesStore.Mail;
using WebSiteClothesStore.Models;
using WebSiteClothesStore.ModelTemp;

namespace WebSiteClothesStore.Controllers
{
    public class CardController : Controller
    {
        // GET: Card

        // GET: AddProduct

        static int maHoaDon = 0;
        static double tongGiaTienHoaDon = 0;
        MydataContext db = new MydataContext();
        public List<ItemCardTemp> GetGioHang()
        {
            //giỏ hàng đã tồn tại
            List<ItemCardTemp> listGioHang = Session["GioHangTam"] as List<ItemCardTemp>;
            if (listGioHang == null)
            {
                //Nếu giỏ  hàng chưa tồn tại thì khởi tạo
                listGioHang = new List<ItemCardTemp>();
                Session["GioHangTam"] = listGioHang; // session thay đổi theo list khi dữ liệu thay đổi
            }
            return listGioHang;
        }
        [HttpPost]
        public ActionResult AddProCard(int? maSP, int sLMua, int? maSize)
        {
            int maDH;
            BangSanPham sp = db.BangSanPhams.SingleOrDefault(p => p.MaSP == maSP);
            CTSanPham ctSP = db.CTSanPhams.SingleOrDefault(p => p.MaCT == maSize);
            if (sp == null || ctSP == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            
            if (Session["TaiKhoan"] == null)// nếu chưa đăng nhập
            {
                List<ItemCardTemp> listCard = GetGioHang();
                ItemCardTemp proExist = listCard.SingleOrDefault(p => p.MaSP == maSP && p.MaCT == maSize);
                if (proExist != null)// nếu đã đã có tìa khoản trong giỏ hàng
                {
                    proExist.SoLuong += sLMua;
                    proExist.ThanhTien = proExist.SoLuong * proExist.DonGia;
                }
                else
                {
                    ItemCardTemp itemCard = new ItemCardTemp(maSP, maSize);
                    if (listCard.Count() == 0)
                    {// chưa có sản phẩm
                        itemCard.MaCTDDH = 1;
                    }
                    else
                    {
                        itemCard.MaCTDDH = listCard.Count() + 1;
                    }

                    listCard.Add(itemCard);
                }
                var tongSoLuong = listCard.Sum(p => p.SoLuong);
                ViewBag.GioHangTam = listCard;
                return Content(tongSoLuong.ToString());
            }
            else// nếu đã đăng nhập
            {
                if (Session["GioHang"] == null)// tạo đơn hàng
                {
                    DonDatHang donHang = new DonDatHang();
                    donHang.DaDat = false;
                    if (Session["TaiKhoan"] != null)
                    {
                        ThanhVien tv = (ThanhVien)Session["TaiKhoan"];

                        //donHang.MaKH = db.KhachHangs.FirstOrDefault(p => p.MaTV == tv.MaTV).MaKH;
                        KhachHang client = db.KhachHangs.FirstOrDefault(p => p.MaTV == tv.MaTV);
                        if (client != null)
                        {
                            donHang.MaKH = client.MaKH;
                        }
                    }
                    db.DonDatHangs.Add(donHang);
                    db.SaveChanges();
                    Session["GioHang"] = donHang;


                }// đã có
                DonDatHang dh = (DonDatHang)Session["GioHang"];
                maDH = dh.MaDDH;
                var checkPro = db.CTDonDatHangs.FirstOrDefault(p => p.MaSP == maSP && p.MaDDH == maDH && p.MaCTSP == maSize);
                if (checkPro != null)// nếu có sản phẩm
                {
                    checkPro.SoLuong += sLMua;
                    db.SaveChanges();
                }
                else
                {
                    CTDonDatHang ctDonDatHang = new CTDonDatHang();
                    ctDonDatHang.MaDDH = maDH;
                    ctDonDatHang.MaSP = (int)maSP;
                    ctDonDatHang.SoLuong = sLMua;
                    ctDonDatHang.MaCTSP = maSize;
                    var product = db.BangSanPhams.FirstOrDefault(p => p.MaSP == maSP);
                    ctDonDatHang.TenSP = product.TenSP;
                    ctDonDatHang.DonGia = (decimal)((double)product.Dongia - ((double)product.Dongia * product.GiamGia / 100));
                    db.CTDonDatHangs.Add(ctDonDatHang);
                    db.SaveChanges();

                }
                var tongSoLuong = db.CTDonDatHangs.Where(p => p.MaDDH == maDH).Sum(p => p.SoLuong);
                return Content(tongSoLuong.ToString());
            }
        }

        public ActionResult ShowCardProduct()
        {
            if (Session["TaiKhoan"] == null)// nếu khách chưa có đăng nhập
            {
                var listCard = GetGioHang();
                ViewBag.GioHangTam = listCard;
                ViewBag.TongThanhTien = listCard.Sum(p => p.SoLuong * p.ThanhTien);
            }
            else
            {
                if (Session["GioHang"] != null)
                {
                    var card = (DonDatHang)Session["GioHang"];
                    var listDetailCart = db.CTDonDatHangs.Where(p => p.MaDDH == card.MaDDH);

                    ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                    KhachHang khachHang = db.KhachHangs.FirstOrDefault(p => p.MaTV == tv.MaTV);
                    ViewBag.KhachHang = khachHang;
                    ViewBag.GioHangCSDL = listDetailCart;
                    ViewBag.TongThanhTien = listDetailCart.Sum(p => p.SoLuong * p.DonGia);
                }
            }
           
            return View("ShowCardProduct");

        }
        public ActionResult PlusCountCard(int maCTDDH)
        {
            string status = null;
            if (Session["TaiKhoan"] != null)
            {
                var productUpdate = db.CTDonDatHangs.FirstOrDefault(p => p.MaCTDDH == maCTDDH);
                productUpdate.SoLuong++;
                var countExist = db.CTSanPhams.FirstOrDefault(p => p.MaCT == productUpdate.MaCTSP);
                if (countExist != null)
                {
                    if (countExist.SoLuongTon > productUpdate.SoLuong)
                    {
                        db.SaveChanges();
                        var tongSoLuong = db.CTDonDatHangs.Where(p => p.MaDDH == productUpdate.MaDDH).Sum(p => p.SoLuong);
                        ViewBag.TongSoLuong = tongSoLuong;
                        return Content(productUpdate.SoLuong.ToString());
                    }
                    else
                    {
                        status = "Không đủ số lượng sản phẩm ";
                        return Content(status);
                    }
                }
                else
                {
                    status = "Sản phẩm không tồn tại";
                    return Content(status);
                }
            }
            else
            {
                List<ItemCardTemp> listItemCard = GetGioHang();
                var productUpdateTemp = listItemCard.FirstOrDefault(p => p.MaCTDDH == maCTDDH);
                productUpdateTemp.SoLuong++;
                var countExistTemp = db.CTSanPhams.FirstOrDefault(p => p.MaCT == productUpdateTemp.MaCT);
                if (countExistTemp != null)
                {
                    if (countExistTemp.SoLuongTon > productUpdateTemp.SoLuong)
                    {
                        var tongSoLuong = listItemCard.Sum(p => p.SoLuong);
                        ViewBag.TongSoLuong = tongSoLuong;
                        return Content(productUpdateTemp.SoLuong.ToString());
                    }
                    else
                    {
                        productUpdateTemp.SoLuong--;
                        status = "Không đủ số lượng sản phẩm ";
                        return Content(status);
                    }
                }
                else
                {
                    status = "Sản phẩm không tồn tại";
                    return Content(status);
                }
            }
        }
        public ActionResult MinusCountCard(int maCTDDH)
        {
            string status = null;
            if (Session["TaiKhoan"] != null)
            {
                var productUpdate = db.CTDonDatHangs.FirstOrDefault(p => p.MaCTDDH == maCTDDH);
                productUpdate.SoLuong--;
                var countExist = db.CTSanPhams.FirstOrDefault(p => p.MaCT == productUpdate.MaCTSP);
                if (countExist != null)
                {
                    if (countExist.SoLuongTon > productUpdate.SoLuong)
                    {
                        db.SaveChanges();
                        var tongSoLuong = db.CTDonDatHangs.Where(p => p.MaDDH == productUpdate.MaDDH).Sum(p => p.SoLuong);
                        ViewBag.TongSoLuong = tongSoLuong;
                        return Content(productUpdate.SoLuong.ToString());
                    }
                    else
                    {
                        status = "Không đủ số lượng sản phẩm ";
                        return Content(status);
                    }
                }
                else
                {
                    status = "Sản phẩm không tồn tại";
                    return Content(status);
                }
            }
            else
            {
                List<ItemCardTemp> listItemCard = GetGioHang();
                var productUpdateTemp = listItemCard.FirstOrDefault(p => p.MaCTDDH == maCTDDH);
                productUpdateTemp.SoLuong--;
                var countExistTemp = db.CTSanPhams.FirstOrDefault(p => p.MaCT == productUpdateTemp.MaCT);
                if (countExistTemp != null)
                {
                    if (countExistTemp.SoLuongTon > productUpdateTemp.SoLuong)
                    {
                        var tongSoLuong = listItemCard.Sum(p => p.SoLuong);
                        ViewBag.TongSoLuong = tongSoLuong;
                        return Content(productUpdateTemp.SoLuong.ToString());
                    }
                    else
                    {
                        status = "Không đủ số lượng sản phẩm ";
                        return Content(status);
                    }
                }
                else
                {
                    status = "Sản phẩm không tồn tại";
                    return Content(status);
                }
            }
        }
        public ActionResult DeleteProductInDetailCard(int maSP, int maCTDDH, int maCTSP)
        {
            if (Session["TaiKhoan"] != null)
            {
                var productIsDeleted = db.CTDonDatHangs.FirstOrDefault(p => p.MaCTDDH == maCTDDH && p.MaCTSP == maCTSP && p.MaSP == maSP);
                List<CTDonDatHang> listDetailCart;
                if (productIsDeleted != null)
                {
                    int maDH = (int)productIsDeleted.MaDDH;
                    db.CTDonDatHangs.Remove(productIsDeleted);
                    db.SaveChanges();
                    listDetailCart = db.CTDonDatHangs.Where(p => p.MaDDH == maDH).ToList();
                }
            } else
            {
                var listItemCard = GetGioHang();
                var productDeleteed = listItemCard.FirstOrDefault(p => p.MaCT == maCTSP && p.MaCTDDH == maCTDDH && p.MaSP == maSP);
                listItemCard.Remove(productDeleteed);
            }
            return RedirectToAction("ShowCardProduct");// gọi lại action ShowCardProduct
        }

        
        public ActionResult OrderProducts(KhachHang kh)
        {
   
            if (Session["KhachHang"] != null)
            {
                KhachHang client = Session["KhachHang"] as KhachHang;

                client.TenKH = kh.TenKH;
                client.DiaChi = kh.DiaChi;
                client.SDT = kh.SDT;
                client.Email = kh.Email;

                db.SaveChanges();
                if (Session["TaiKhoan"] == null)
                {
                    var listCard = GetGioHang();
                    ViewBag.GioHangTam = listCard;

                    tongGiaTienHoaDon = listCard.Sum(p => p.SoLuong * p.DonGia);
                    ViewBag.TongThanhTien = tongGiaTienHoaDon;
                }
                else
                {
                    ViewBag.KhachHang = client;
                    ViewBag.GioHangCSDL = db.CTDonDatHangs.Where(p => p.MaDDH == maHoaDon);
                    tongGiaTienHoaDon = (double)db.CTDonDatHangs.Where(p => p.MaDDH == maHoaDon).Sum(p => p.SoLuong * p.DonGia);
                    ViewBag.TongThanhTien = tongGiaTienHoaDon;
                }
                Session["KhachHang"] = client;
                return View();
               
            }    

            Session["KhachHang"] = kh;

            if (Session["GioHang"] == null && Session["GioHangTam"]==null)
            {
                return RedirectToAction("Index", "HomeWeb");
            }
            if (Session["TaiKhoan"] != null)
            {
                ThanhVien tv = Session["TaiKhoan"] as ThanhVien;
                KhachHang client = db.KhachHangs.FirstOrDefault(p => p.MaTV == tv.MaTV);
                if (client != null)
                {
                    client.TenKH = tv.HoTen;
                    client.DiaChi = tv.DiaChi;
                    client.SDT = tv.SDT;
                    client.Email = tv.Email;

                    db.SaveChanges();
                    var cardOdered = db.DonDatHangs.FirstOrDefault(p => p.MaKH == client.MaKH && p.DaDat == false);
                    if (cardOdered != null)
                    {// có đơn => cập nhật lại trạng thái đặt
                        cardOdered.MaKH = client.MaKH;
                        cardOdered.NgayDat = DateTime.Now;
                       
                        cardOdered.TinhTrangDDH = "Đang Giao";
                        cardOdered.DaThanhToan = false;
                        cardOdered.UuDai = 0;
                        //cardOdered.DaDat = true;
                        cardOdered.DaHuy = false;

                        maHoaDon = cardOdered.MaDDH;
                        tongGiaTienHoaDon = (double)db.CTDonDatHangs.Where(p => p.MaDDH == maHoaDon).Sum(p => p.SoLuong * p.DonGia);
                        db.SaveChanges();


                        ViewBag.KhachHang = client;
                        ViewBag.GioHangCSDL = db.CTDonDatHangs.Where(p => p.MaDDH == maHoaDon);
                        ViewBag.TongThanhTien = tongGiaTienHoaDon;
                        //Session["GioHang"] = null;
                        //ViewBag.GioHangCSDL = null;
                    }
                }
            }
            else // khách không phải là thành viên
            {
                KhachHang newClient = new KhachHang();
                newClient = kh;

                db.KhachHangs.Add(newClient);
                db.SaveChanges();

                //Thêm đơn hàng
                DonDatHang donDH = new DonDatHang();
                donDH.MaKH = newClient.MaKH;
                donDH.NgayDat =DateTime.Now;
               
                donDH.TinhTrangDDH = "Đang Giao";
                donDH.DaThanhToan = false;
                donDH.UuDai = 0;
                donDH.DaHuy = false;
                //donDH.DaDat = true;
                db.DonDatHangs.Add(donDH);
                db.SaveChanges();
                maHoaDon = donDH.MaDDH;
                List<ItemCardTemp> listGH = GetGioHang(); // nhận giá trị từ session
                foreach (var item in listGH)
                {
                    CTDonDatHang ctddh = new CTDonDatHang();
                    ctddh.MaDDH = donDH.MaDDH;
                    ctddh.MaSP = item.MaSP;
                    ctddh.MaCTSP = item.MaCT;
                    ctddh.TenSP = item.TenSP;
                    ctddh.SoLuong = item.SoLuong;
                    ctddh.DonGia = (decimal)item.DonGia;
                    db.CTDonDatHangs.Add(ctddh);
                    db.SaveChanges();
                }
               
                ViewBag.GioHangTam = listGH;

                tongGiaTienHoaDon = listGH.Sum(p => p.SoLuong * p.DonGia);
                ViewBag.TongThanhTien = tongGiaTienHoaDon;

                //Session["GioHangTam"] = null;
                //ViewBag.GioHangTam = null;

            }
            //List<CTDonDatHang> listDetailProduct = db.CTDonDatHangs.Where(p => p.MaDDH == maDH).ToList();
            //foreach(var item in listDetailProduct)
            //{
            //    BangSanPham productUpdateNumberBuys = db.BangSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP);
            //    if(productUpdateNumberBuys!=null)
            //        productUpdateNumberBuys.SoLanMua+=item.SoLuong;
            //    CTSanPham productUpdateCount = db.CTSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP && p.MaCT == item.MaCTSP);
            //    if(productUpdateCount!=null)
            //        productUpdateCount.SoLuongTon -= item.SoLuong;
            //    db.SaveChanges();
            //}

            return View();
         }


       
        public ActionResult Payment()
        {
            KhachHang kh = Session["KhachHang"] as KhachHang;
            var hoaDon = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maHoaDon);

            MailMessage mSG = new MailMessage();
            AlternateView plainView = AlternateView.CreateAlternateViewFromString("Some plaintext", Encoding.UTF8, "text/plain");
            mSG.AlternateViews.Add(plainView);
            mSG.From = new MailAddress(MyEmail.name, "Thông báo từ Shop nhóm 4");
            mSG.To.Add(kh.Email); // thêm địa chỉ mail người nhận
            mSG.Subject = "Bạn có một đơn hàng, vui lòng chọn xác nhận để có thể tiến hành lên đơn";// Thêm tiêu đề mail;
            string tenKH = kh.TenKH;

            string style = " color:black; font-size:20px; font-weight:900; background-color:greenyellow; padding:10px 50px; text-decoration:none; border-radius:20px;";
            string test = string.Format(@"""{0}""", style);
            string link = string.Format(@"""{0}""", "https://localhost:44331/Card/ConfirmCard?maDH=" + maHoaDon + "");
            string sttykeTheA = string.Format(@"""{0}""", "b");
            string displayInline = string.Format(@"""{0}""", "display:inline-block;");
            //padding:10px 20px; background - color:greenyellow;text - decoration:none;border - radius:20px;color: black; font - size:16px;font - weight:900; 
            string htmlText = $"<b style={ displayInline}>Đơn hàng của (anh/chị) :</b> <h3 style={ displayInline}>{tenKH}</h3> <b style={ displayInline}>Có giá trị là  :</b><h3 style={ displayInline}>{tongGiaTienHoaDon.ToString("#,##")} VNĐ </h3> <b style={ displayInline}> được đặt vào ngày : </b><h3 style={ displayInline}>{hoaDon.NgayDat}</h3>  " +
                $"<a href={link} style={test} target='_blank'>Xác nhận đơn hàng </a>";

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
          
            return RedirectToAction("ShowCardUser", "Home");
        }
        public ActionResult ConfirmCard(int maDH)
        {
            if (maDH == 0)
            {
                return View("ThongBaoKhongThanhCong");
            }
            List<CTDonDatHang> listDetailProduct = db.CTDonDatHangs.Where(p => p.MaDDH == maDH).ToList();
            if (listDetailProduct.Count() == 0)
            {
                return View("ThongBaoKhongThanhCong");
            }
            DonDatHang donDatHang = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maDH);
            int soLanMua = db.KhachHangs.Where(p => p.MaKH == donDatHang.MaKH).Count();
            KhachHang kh = db.KhachHangs.FirstOrDefault(p => p.MaKH == donDatHang.MaKH);
            ThanhVien tv = db.ThanhViens.FirstOrDefault(p => p.MaTV == kh.MaTV);
            int uuDai = 0;
            if (tv != null)
            {
                if (soLanMua > 0 && soLanMua < 5)
                {
                    tv.MaLoaiTV = 1;
                }
                if (soLanMua > 5 && soLanMua < 10)
                {
                    tv.MaLoaiTV = 2;
                    uuDai = 10;
                }
                if (soLanMua > 10 && soLanMua < 15)
                {
                    tv.MaLoaiTV = 3;
                    uuDai = 15;
                }
                if (soLanMua > 15 && soLanMua < 20)
                {
                    tv.MaLoaiTV = 4;
                    uuDai = 20;
                }
                if (soLanMua > 20)
                {
                    tv.MaLoaiTV = 5;
                    uuDai = 25;
                }
            }
            if (donDatHang != null)
            {
                donDatHang.DaDat =true;
                donDatHang.NgayGiao = DateTime.Now.AddDays(4);
                donDatHang.DaThanhToan = false;
                donDatHang.UuDai = uuDai;
                db.SaveChanges();
            }
            foreach (var item in listDetailProduct)
            {
                BangSanPham productUpdateNumberBuys = db.BangSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP);
                if (productUpdateNumberBuys != null)
                    productUpdateNumberBuys.SoLanMua += item.SoLuong;
                CTSanPham productUpdateCount = db.CTSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP && p.MaCT == item.MaCTSP);
                if (productUpdateCount != null)
                    productUpdateCount.SoLuongTon -= item.SoLuong;
                db.SaveChanges();
            }
            Session["GioHang"] = null;
            ViewBag.GioHangCSDL = null;
            Session["GioHangTam"] = null;
            ViewBag.GioHangTam = null;
            return RedirectToAction("ThongBaoDatHang", "Card");
        }


        public ActionResult PaymentByMomo(FormCollection collection)
        {
            KhachHang kh = Session["KhachHang"] as KhachHang;
            var hoaDon = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maHoaDon);
            

            string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
            string partnerCode = "MOMOIP1R20220408";
            string accessKey = "2Rq8mwC0AtwgOHBR";
            string serectkey = "UavKQ3fniXWMUwElozhdr67CrfTas1TC";
            string orderInfo = DateTime.Now.ToString();
            string returnUrl = "https://webhook.site/b3088a6a-2d17-4f8d-a383-71389a6c600b";// nhận đường dẫn từ momo
            string ipnUrl = "https://localhost:44331/Card/PaymentMomo";
            string redirectUrl = "https://localhost:44331/Card/ReturnUrl";
            string requestType = "captureWallet";

            string amount = tongGiaTienHoaDon.ToString() ;
            string orderId = Guid.NewGuid().ToString();
            string requestId = Guid.NewGuid().ToString();
            string extraData = "";

            ////Before sign HMAC SHA256 signature
            string rawHash = "accessKey=" + accessKey +
                 "&amount=" + amount +
                 "&extraData=" + extraData +
                 "&ipnUrl=" + ipnUrl +
                 "&orderId=" + orderId +
                 "&orderInfo=" + orderInfo +
                 "&partnerCode=" + partnerCode +
                 "&redirectUrl=" + redirectUrl +
                 "&requestId=" + requestId +
                 "&requestType=" + requestType
                 ;



            MoMoSecurity crypto = new MoMoSecurity();
            ////sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            ////build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "partnerName", "Test" },
                { "storeId", "MomoTestStore" },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderId },
                { "orderInfo", orderInfo },
                { "redirectUrl", redirectUrl },
                { "ipnUrl", ipnUrl },
                { "lang", "en" },
                { "extraData", extraData },
                { "requestType", requestType },
                { "signature", signature }

            };
            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

           


            return Redirect(jmessage.GetValue("payUrl").ToString());
        }

        
        public ActionResult ReturnUrl()
        {
            string status = Request["message"].ToString();
            if(status.Contains("Successful"))
            {
                List<CTDonDatHang> listDetailProduct = db.CTDonDatHangs.Where(p => p.MaDDH == maHoaDon).ToList();
                if (listDetailProduct.Count() == 0)
                {
                    return View("ThongBaoKhongThanhCong");
                }

                DonDatHang donDatHang = db.DonDatHangs.FirstOrDefault(p => p.MaDDH == maHoaDon);
                int soLanMua = db.DonDatHangs.Where(p => p.MaKH == donDatHang.MaKH).Count();
                KhachHang kh = db.KhachHangs.FirstOrDefault(p => p.MaKH == donDatHang.MaKH);
                ThanhVien tv = db.ThanhViens.FirstOrDefault(p => p.MaTV == kh.MaTV);
                int uuDai = 0;
                if (tv != null)
                {
                    if(soLanMua>0 && soLanMua < 5)
                    {
                        tv.MaLoaiTV = 1;
                    }
                    if (soLanMua > 5 && soLanMua < 10)
                    {
                        tv.MaLoaiTV = 2;
                        uuDai = 10;
                    }
                    if (soLanMua > 10 && soLanMua < 15)
                    {
                        tv.MaLoaiTV = 3;
                        uuDai = 15;
                    }
                    if (soLanMua > 15 && soLanMua < 20)
                    {
                        tv.MaLoaiTV = 4;
                        uuDai = 20;
                    }
                    if (soLanMua > 20)
                    {
                        tv.MaLoaiTV = 5;
                        uuDai = 25;
                    }
                }
                db.SaveChanges();
                if (donDatHang != null)
                {
                    donDatHang.DaDat = true;
                    donDatHang.NgayGiao = DateTime.Now.AddDays(4);
                    donDatHang.DaThanhToan = true;
                    donDatHang.TinhTrangDDH = "Đang giao";
                    donDatHang.UuDai =uuDai;
                    db.SaveChanges();
                }
                foreach (var item in listDetailProduct)
                {
                    BangSanPham productUpdateNumberBuys = db.BangSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP);
                    if (productUpdateNumberBuys != null)
                        productUpdateNumberBuys.SoLanMua += item.SoLuong;
                    CTSanPham productUpdateCount = db.CTSanPhams.FirstOrDefault(p => p.MaSP == item.MaSP && p.MaCT == item.MaCTSP);
                    if (productUpdateCount != null)
                        productUpdateCount.SoLuongTon -= item.SoLuong;
                    db.SaveChanges();
                }
                Session["GioHang"] = null;
                ViewBag.GioHangCSDL = null;
                Session["GioHangTam"] = null;
                ViewBag.GioHangTam = null;
                return View("ThongBaoDatHang");
            }
            else
            {
                return View("ThongBaoKhongThanhCong");
            }
        }
           

        [HttpPost]
        public ActionResult SavePayment()
        {
          

            return View("ShowCardProduct");

        }
        public ActionResult ThongBaoDatHang()
        {
            return View();
        }
        public ActionResult ThongBaoKhongThanhCong()
        {
            return View();
        }
        public ActionResult VoteForProduct(int maCTDH , string status)
        {
            int like=0;
            if (status == "Yes")
            {
                like = 1;
                status = "Cảm ơn bạn đã thích sản phẩm";
            }
            else if(status=="No")
            {
                like = 0;
                status = "Xin lỗi bạn vì trãi nghiệm vừa rồi";
            }
            var updateVoteProduct = db.CTDonDatHangs.FirstOrDefault(p => p.MaCTDDH == maCTDH);
            if (updateVoteProduct != null)
            {
                updateVoteProduct.BinhChon = like;
                var updateBinhChonPro = db.BangSanPhams.FirstOrDefault(p => p.MaSP == updateVoteProduct.MaSP);
                updateBinhChonPro.LuotBinhChon += 1;
                db.SaveChanges();
            }
            return Content(status);

        }
    }
}












