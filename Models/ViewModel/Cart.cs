using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace _24DH110165_MyStore.Models.ViewModel
{
    public class Cart
    {
        private List<CartItem> items = new List<CartItem>();
        public IEnumerable<CartItem> Items => items;
        
        //Danh sách sản phẩm trong giỏ hàng được nhóm theo Category
        public List<IGrouping<string, CartItem>> GroupedItems => items.GroupBy(i => i.Category).ToList();
        
        //Thuộc tính hỗ trợ phân trang
        public int PageNumber { get; set; } //Trang hiện tại
        public int PageSize { get; set; } = 6; //Số sản phẩm mỗi trang
        
        //Danh sách các sản phẩm cùng danh mục với các sản phẩm trong giỏ hàng
        public PagedList.IPagedList<Product> SimilarProducts { get; set; }

        //Thêm sản phẩm vào giỏ
        public void AddItem(int productID, string productImage, string productName,
            decimal unitPrice, int quantity, string category)
        {
            var existingItem = items.FirstOrDefault(i => i.ProductID == productID);
            if (existingItem == null)
            {
                items.Add(new CartItem { ProductID = productID, ProductImage = productImage,
                    ProductName = productName, UnitPrice = unitPrice, Quantity = quantity });
            }
            else
            {
                existingItem.Quantity += quantity;
            }
        }
        //xóa sản phẩm khỏi giỏ
        public void RemoveItem(int productID)
        {
            items.RemoveAll(i => i.ProductID == productID);
        }
        //tính tổng giá trị giỏ hàng
        public decimal TotalValue()
        {
            return items.Sum(i => i.TotalPrice);
        }
        //Làm trống giỏ hàng
        public void Clear()
        { 
            items.Clear();
        }
        //Cập nhật số lượng của sản phẩm đã chọn
        public void UpdateQuantity(int productID, int quantity)
        {
            var item = items.FirstOrDefault(i => i.ProductID == productID);
            if (item != null)
            {
                item.Quantity = quantity;
            }
        }
    }
}