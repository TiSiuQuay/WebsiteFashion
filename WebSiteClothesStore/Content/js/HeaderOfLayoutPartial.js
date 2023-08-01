$("#btnDangNhap").click(function () {
    console.log("Vô nè");
    $("#popup-login").slideDown(500);
    //show() hienr thị bình thường
    //fadeIn(500) hiển thị trễ 500ms
    //slideDown(500) hiển thị từ trên xuống 500 ms

});
$("#btnLogin").click(function () {
    var taiKhoan = $(".TaiKhoan").val();//.val() lấy giá trị trong thẻ
    var matKhau = $(".MatKhau").val();
    if (taiKhoan == "") {
        alert("Lỗi");
        return;
    }
})