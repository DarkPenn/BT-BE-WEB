using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using _24DH110165_MyStore.Models;
using _24DH110165_MyStore.Models.ViewModel;
using PagedList;
using PagedList.Mvc;

namespace _24DH110165_MyStore.Areas.Admin.Controllers
{
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products
        public ActionResult Index(string SearchTerm, decimal? MinPrice, decimal? MaxPrice, string SortOrder, int? page)
        {            
            db.Database.CommandTimeout = 180;
            var model = new ProductSearchVM();
            var products = db.Products.AsQueryable();
            //Tìm kiếm theo từ khóa
            if(!string.IsNullOrEmpty(SearchTerm))
            {
                products= products.Where(p=> 
                    p.ProductName.Contains(SearchTerm) || 
                    p.ProductDescription.Contains(SearchTerm) || 
                    p.Category.CategoryName.Contains(SearchTerm));
            }
            //tìm kiếm theo giá tối thiểu
            if (MinPrice.HasValue)
                products = products.Where(p => p.ProductPrice >= MinPrice.Value);
            //tìm kiếm theo giá tối đa
            if (MaxPrice.HasValue)
                products = products.Where(p => p.ProductPrice <= MaxPrice.Value);
            //Sắp xếp theo lựa chọn người dùng
            switch(SortOrder)
            {
                case "name_asc": products = products.OrderBy(p => p.ProductName);
                    break;
                case "name-desc": products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc": products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc": products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductName);
                    break;
            }
            model.SortOrder = SortOrder;

            //Phân trang
            int PageNumber = page ?? 1; //Mặc định là 1 nếu không có giá trị
            int PageSize = 2; // số sản phẩm mỗi trang
            model.Products=products.ToPagedList(PageNumber,PageSize);
            //model.Products=products.ToList();
            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage")] Product product)
        {
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
