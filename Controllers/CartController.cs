using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using _24DH110165_MyStore.Models;
using _24DH110165_MyStore.Models.ViewModel;
using PagedList;

namespace _24DH110165_MyStore.Controllers
{
    public class CartController : Controller
    {
        private MyStoreEntities db=new MyStoreEntities();

        //hàm lấy dịch vụ giỏ hàng
        private CartService GetCartService()
        {
            return new CartService(Session);
        }

        /*Hiển thị giỏ hàng không gom theo nhóm danh mục
        public ActionResult Index()
        {
            var cart = GetCartService().GetCart();
            return View(cart);
        }*/
        //Hiển thị giỏ hàng đã gom nhóm sản phẩm theo danh mục
        public ActionResult Index(int? page)
        {
            var cart = GetCartService().GetCart();
            var products = db.Products.ToList();
            var similarProducts = new List<Product>();

            if (cart.Items != null && cart.Items.Any())
            {
                similarProducts = products.Where(p => cart.Items.Any(ci => ci.Category == p.Category.CategoryName)
                && !cart.Items.Any(ci => ci.ProductID == p.ProductID)).ToList();
            }
            //Lấy trang hiện tại(mặc định là 1 nếu không có giá trị)
            int pageNumber = page ?? 1;
            int pageSize = cart.PageSize; // số sản phẩm mỗi trang

            cart.SimilarProducts = similarProducts.OrderBy(p => p.ProductID).ToPagedList(pageNumber, pageSize);
            return View(cart);
        }
        //thêm sản phẩm vào giỏ
        public ActionResult AddToCart(int id, int quantity = 1)
        {
            var product = db.Products.Find(id);
            if (product != null)
            {
                var cartService = GetCartService();
                cartService.GetCart().AddItem(product.ProductID, product.ProductImage,
                    product.ProductName, product.ProductPrice, quantity, product.Category.CategoryName);
            }
            return RedirectToAction("Index");
        }

        //xóa sản phẩm khỏi giỏ
        public ActionResult RemoveFromCart(int id)
        {
            var cartService= GetCartService();
            cartService.GetCart().RemoveItem(id);
            return RedirectToAction("Index");
        }

        //làm trống giỏ hàng
        public ActionResult ClearCart()
        {
            GetCartService().ClearCart();  
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult UpdateQuantity(int id, int quantity)
        {
            var cartService = GetCartService();
            cartService.GetCart().UpdateQuantity(id, quantity);
            return RedirectToAction("Index");
        }
    }
}