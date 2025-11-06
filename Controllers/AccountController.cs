using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH110165_MyStore.Models.ViewModel;
using _24DH110165_MyStore.Models;
using System.Web.Security;
using System.Runtime.Remoting.Messaging;

namespace _24DH110165_MyStore.Controllers
{
    public class AccountController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }
        //POST: Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                //kiểm tra tên đăng nhập đã tồn tại chưa
                var existingUser = db.Users.SingleOrDefault(u => u.Username == model.Username);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Tên đăng nhập này đã tồn tại:");
                    return View(model);
                }

                //Nếu chưa tồn tại thì tạo bản ghi thông tin tài khoản trong bảng User
                var user = new _24DH110165_MyStore.Models.User
                {
                    Username = model.Username,
                    Password = model.Password,//nên mã hóa mật khẩu trước khi lưu
                    UserRole = "C"
                };
                db.Users.Add(user);
                //và tạo bản ghi thông tin khách hàng trong Customer
                var customer = new _24DH110165_MyStore.Models.Customer
                {
                    CustomerName = model.CustomerName,
                    CustomerEmail = model.CustomerEmail,
                    CustomerPhone = model.CustomerPhone,
                    CustomerAddress = model.CustomerAddress,
                    Username = model.Username,
                };
                db.Customers.Add(customer);
                //lưu thông tin tài khoản và thông tin khách hàng vào CSDL
                db.SaveChanges();

                return RedirectToAction("Index","Home");
            }
            return View(model);
        } 

        //GET: Account/Login
        public ActionResult Login()
        { return View(); }
        //POST: Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == model.Username
                    && u.Password == model.Password
                    && u.UserRole == "C");
                if (user != null)
                {
                    //Lưu trạng thái đăng nhập vào session
                    Session["Username"] = user.Username;
                    Session["UserRole"] = user.UserRole;

                    //Lưu thông tin xác thực người dùng vào cookie
                    FormsAuthentication.SetAuthCookie(user.Username,false);

                    return RedirectToAction("Index", "Home");
                }
                else { ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng."); }
            }
            return View(model);
        }

        //GET: Account/LogOut
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        //GET: Account/ChangePassword
        public ActionResult ChangePassword()
        {
             return View(); 
        }
        //POST: Accout/ChangePassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult ChangePassword(RegisterVM model)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.SingleOrDefault(u => u.Username == User.Identity.Name);
                if (user == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                user.Password = model.Password;//nên mã hóa trước khi lưu
                db.SaveChanges();

                return RedirectToAction("ProfileInfo");
            }
            return View(model);
        }
    }
}
