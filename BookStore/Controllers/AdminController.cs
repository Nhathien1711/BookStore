using BookStore.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
namespace MvcBookStore.Controllers
{
    public class AdminController : Controller
    {
        // Use DbContext to manage database
        QLBANSACHEntities database = new QLBANSACHEntities();
        // GET: Admin
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
                return RedirectToAction("Login");
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(ADMIN admin)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(admin.UserAdmin))
                    ModelState.AddModelError(string.Empty, "User name không được để trống");
            if (string.IsNullOrEmpty(admin.PassAdmin))
                    ModelState.AddModelError(string.Empty, "Password không được để trống");
                    //Kiểm tra có admin này hay chưa
                    var adminDB = database.ADMINs.FirstOrDefault(ad => ad.UserAdmin ==
                    admin.UserAdmin && ad.PassAdmin == admin.PassAdmin);
                if (adminDB == null)
                    ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng");
                else
                {
                    Session["Admin"] = adminDB;

                    ViewBag.ThongBao = "Đăng nhập admin thành công";
                    return RedirectToAction("Index", "Admin");
                }
            }
            return View();

        }
       public ActionResult Sach(int? page)
        {
            var dsSach = database.SACHes.ToList();
            //Tạo biến cho biết số sách mỗi trang
            int pageSize = 7;
            //Tạo biến số trang
            int pageNum = (page ?? 1);
            return View(dsSach.OrderBy(sach => sach.Masach).ToPagedList(pageNum, pageSize));
        }
        //Tạo mới sách
        [HttpGet]
        public ActionResult ThemSach()
        {
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");
            return View();
        }
        [HttpPost]
        public ActionResult ThemSach(SACH sach, HttpPostedFileBase Hinhminhhoa)
        {
            ViewBag.MaCD = new SelectList(database.CHUDEs.ToList(), "MaCD", "TenChuDe");
            ViewBag.MaNXB = new SelectList(database.NHAXUATBANs.ToList(), "MaNXB", "TenNXB");

            if (Hinhminhhoa == null)
            {
                ViewBag.ThongBao = "Vui lòng chọn ảnh bìa";
                return View();
            }    

            else
            {
                if (ModelState.IsValid)//Nếu dữ liệu sách đầy đủ
                {
                    //Lấy tên file của hình được up lên
                    var fileName = Path.GetFileName(Hinhminhhoa.FileName);

                    //Tạo đường dẫn tới file
                    var path = Path.Combine(Server.MapPath("~/Images"), fileName);

                    //Kiểm tra hình đã tồn tại trong hệ thống chưa
                    if (System.IO.File.Exists(path))
                    {
                        ViewBag.ThongBao = "Hình đã tồn tại";
                    }
                    else
                    {
                        Hinhminhhoa.SaveAs(path);//Lưu vào hệ thống
                    }
                    //Lưu tên sách vào trường Hinhminhhhoa
                    sach.Hinhminhhoa = fileName;
                    //Lưu vào CSDL
                    database.SACHes.Add(sach);
                    database.SaveChanges();
                }
                return RedirectToAction("Sach");
            }    
           
        }
        public ActionResult ChiTietSach(int id)
        {
            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null) //Không thấy sách
            {
                Response.StatusCode = 404;
                return null;
            }    
            return View(sach); //Hiển thị thông tin sách cần
        }
        [HttpGet]
        public ActionResult SuaSach(int id)
        {
            if (id <= 0)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest); // Trả về lỗi 400 nếu id không hợp lệ
            }

            var sach = database.SACHes.FirstOrDefault(s => s.Masach == id);
            if (sach == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy sách
            }

            return View(sach); // Truyền sách vào view
        }

        // POST: Sach/SuaSach/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SuaSach(SACH sACH)
        {
            if (sACH == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest); // Trả về lỗi 400 nếu sACH null
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Đánh dấu đối tượng là đã chỉnh sửa và lưu các thay đổi
                    database.SACHes.Add(sACH);
                    database.Entry(sACH).State = EntityState.Modified;
                    database.SaveChanges();

                    TempData["SuccessMessage"] = "Thông tin sách đã được cập nhật thành công!";
                    return RedirectToAction("Sach"); // Chuyển hướng về trang danh sách
                }
                catch (System.Exception)
                {
                    // Log lỗi (có thể sử dụng logging framework)
                    ModelState.AddModelError("", "Đã xảy ra lỗi trong quá trình cập nhật dữ liệu. Vui lòng thử lại.");
                }
            }

            return View(sACH); // Hiển thị lại form với dữ liệu đã nhập nếu không hợp lệ hoặc gặp lỗi
        }

        // GET: Admin/XoaSach/5
        public ActionResult XoaSach(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm sách theo ID
            SACH sach = database.SACHes.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }

            // Trả về View để xác nhận việc xóa
            return View(sach);
        }

        // POST: Admin/XoaSach/5
        [HttpPost, ActionName("XoaSach")]
        [ValidateAntiForgeryToken]
        public ActionResult XoaSach(int id)
        {
            // Tìm sách theo ID
            SACH sach = database.SACHes.Find(id);
            if (sach != null)
            {
                // Xóa sách khỏi cơ sở dữ liệu
                try
                {
                    database.SACHes.Remove(sach);
                    database.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    // Ghi lại lỗi hoặc thông báo cho người dùng
                    Console.WriteLine(ex.InnerException?.Message);
                    // Hoặc xử lý lỗi theo cách bạn muốn
                }

            }

            // Chuyển hướng về danh sách
            return RedirectToAction("Sach");
        }
    }
}