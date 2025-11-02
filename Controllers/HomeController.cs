using _24DH110165_MyStore.Models;
using _24DH110165_MyStore.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;

namespace _24DH110165_MyStore.Controllers
{
    public class HomeController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string SearchTerm, int? page)
        {
            db.Database.CommandTimeout = 180;
            var model = new HomeProductVM();
            var products = db.Products.AsQueryable();
            //Tìm kiếm theo từ khóa
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                model.SearchTerm = SearchTerm;
                products = products.Where(p =>
                    p.ProductName.Contains(SearchTerm) ||
                    p.ProductDescription.Contains(SearchTerm) ||
                    p.Category.CategoryName.Contains(SearchTerm));
            }
            //Phân trang
            int PageNumber = page ?? 1; //Mặc định là 1 nếu không có giá trị
            int PageSize = 6; // số sản phẩm mỗi trang

            //lấy top10 sản phẩm bán chạy nhất
            model.FeaturedProducts= products.OrderByDescending(p => p.OrderDetails.Count()).Take(10).ToList();
            //lấy 20 sản phẩm ế và ít lượt mua nhất
            model.NewProducts=products.OrderBy(p => p.OrderDetails.Count()).Take(20).ToPagedList(PageNumber,PageSize);
            return View(model);
        }

        // GET: HOME/ProductDetails/5
        public ActionResult ProductDetails(int? id,int? quantity,int? page)
        {
            if(id==null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            Product pro= db.Products.Find(id);
            if (pro==null)
                return HttpNotFound();

            //lấy tất cả sản phẩm cùng danh mục
            var products=db.Products.Where(p => p.CategoryID == pro.CategoryID && p.ProductID!=pro.ProductID).AsQueryable();
            ProductDetailsVM model=new ProductDetailsVM();

            //đoạn code liên quan đến phân trang
            int PageNumber = page ?? 1; //Mặc định là 1 nếu không có giá trị
            int PageSize=model.PageSize; //số sản phẩm mỗi trang
            model.product = pro;
            model.RelatedProducts = products.OrderBy(p => p.ProductID).Take(8).ToPagedList(PageNumber,PageSize);
            model.TopProducts = products.OrderByDescending(p => p.OrderDetails.Count()).Take(8).ToPagedList(PageNumber,PageSize);
            if(quantity.HasValue)
                model.quantity=quantity.Value;

            return View(model);
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